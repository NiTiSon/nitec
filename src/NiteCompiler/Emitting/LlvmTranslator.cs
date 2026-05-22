using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.Emitting;

internal sealed partial class LlvmTranslator
{
	private readonly LLVMContextRef _context;
	private readonly LLVMModuleRef _module;
	private readonly LLVMBuilderRef _builder;
	private readonly NiteCompilation _compilation;
	private readonly BindingDiagnosticBag _diagnostics;
	private readonly Dictionary<FunctionSymbol, FunctionPlan> _plans = new();
	private readonly Dictionary<NamedTypeSymbol, LLVMTypeRef> _structTypes = new();

	private LlvmTranslator(NiteCompilation compilation, string moduleName, BindingDiagnosticBag diagnostics)
	{
		_context = LLVMContextRef.Global;
		_module = _context.CreateModuleWithName(moduleName);
		_builder = _context.CreateBuilder();
		_compilation = compilation;
		_diagnostics = diagnostics;
	}

	public static LLVMModuleRef Translate(NiteCompilation compilation, ImmutableArray<LibrarySymbol> libraries, FunctionSymbol? entryPoint,
		BindingDiagnosticBag diagnostics)
	{
		string moduleName = libraries.IsDefaultOrEmpty ? "__nite" : libraries[0].Name;
		LlvmTranslator translator = new(compilation, moduleName, diagnostics);
		translator.TranslateImpl(libraries, entryPoint);
		translator._builder.Dispose();
		return translator._module;
	}

	private void TranslateImpl(ImmutableArray<LibrarySymbol> libraries, FunctionSymbol? entryPoint)
	{
		foreach (FunctionSymbol function in CollectMonomorphizedFunctions(libraries, entryPoint))
		{
			DeclareFunction(function);
		}

		foreach (var (_, plan) in _plans)
		{
			EmitFunctionBody(plan);
		}

		EmitStaticFields(libraries);

		// TODO: this is shi
		LLVMValueRef fltUsed = _module.AddGlobal(_context.Int32Type, "_fltused");
		fltUsed.IsGlobalConstant = true;
		fltUsed.Initializer = LLVMValueRef.CreateConstInt(_context.Int32Type, 0);

		if (!_module.TryVerify(LLVMVerifierFailureAction.LLVMReturnStatusAction, out string message))
		{
			// TODO: diagnostics
			Debug.WriteLine("LlvmTranslator: " + message);
		}
	}

	private IEnumerable<FunctionSymbol> CollectMonomorphizedFunctions(ImmutableArray<LibrarySymbol> libraries, FunctionSymbol? entryPoint)
	{
		HashSet<FunctionSymbol> discovered = [];
		HashSet<FunctionSymbol> emitted = [];
		Queue<FunctionSymbol> worklist = new();

		foreach (LibrarySymbol library in libraries)
		{
			CollectModuleFunctions(library.GlobalModule, worklist, discovered);
		}

		if (entryPoint != null && discovered.Add(entryPoint))
		{
			worklist.Enqueue(entryPoint);
		}

		while (worklist.Count > 0)
		{
			FunctionSymbol function = worklist.Dequeue();
			if (!emitted.Add(function))
			{
				continue;
			}

			BuildFunctionPlan(function);
			yield return function;

			if (!_plans.TryGetValue(function, out FunctionPlan? plan) || plan.Nir == null)
			{
				continue;
			}

			foreach (FunctionSymbol callee in EnumerateCalledFunctions(plan.Nir))
			{
				if (discovered.Add(callee))
				{
					worklist.Enqueue(callee);
				}
			}
		}
	}

	private static IEnumerable<FunctionSymbol> EnumerateCalledFunctions(NirFunction nir)
	{
		foreach (NirBlock block in nir.Blocks.Values)
		{
			foreach (Instruction instruction in block.Instructions)
			{
				if (instruction is CallInstruction call)
				{
					yield return call.Function;
				}
			}
		}
	}

	private static void CollectModuleFunctions(ModuleSymbol module, Queue<FunctionSymbol> worklist, HashSet<FunctionSymbol> discovered)
	{
		foreach (Symbol member in module.GetMembersUnordered())
		{
			switch (member)
			{
				case FunctionSymbol function when discovered.Add(function):
					worklist.Enqueue(function);
					break;
				case ModuleSymbol nestedModule:
					CollectModuleFunctions(nestedModule, worklist, discovered);
					break;
				case NamedTypeSymbol namedType:
					CollectTypeFunctions(namedType, worklist, discovered);
					break;
			}
		}
	}

	private static void CollectTypeFunctions(NamedTypeSymbol type, Queue<FunctionSymbol> worklist, HashSet<FunctionSymbol> discovered)
	{
		foreach (Symbol member in type.GetMembers())
		{
			switch (member)
			{
				case FunctionSymbol fn when discovered.Add(fn):
					worklist.Enqueue(fn);
					break;
				case NamedTypeSymbol nested:
					CollectTypeFunctions(nested, worklist, discovered);
					break;
			}
		}
	}

	private void BuildFunctionPlan(FunctionSymbol function)
	{
		if (_plans.ContainsKey(function))
		{
			return;
		}

		if (function.IsExtern)
		{
			_plans[function] = new FunctionPlan(function, cfg: null, nir: null);
			return;
		}

		BoundBlock? body = BindFunctionBody(function);
		if (body == null)
		{
			_plans[function] = new FunctionPlan(function, cfg: null, nir: null);
			return;
		}

		ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(function, body, _diagnostics);
		NirFunction nir = NirBuilder.Build(cfg, function);
		_plans[function] = new FunctionPlan(function, cfg, nir);
	}

	private BoundBlock? BindFunctionBody(FunctionSymbol function)
	{
		Binder? binder = null;
		SyntaxNode syntax;

		if (function is SourceFunctionSymbol sourceFunction)
		{
			binder = sourceFunction.TryGetBodyBinder();
			syntax = sourceFunction.Syntax;
		}
		else if (function is SourceConstructorSymbol sourceConstructor)
		{
			binder = sourceConstructor.TryGetBodyBinder();
			syntax = sourceConstructor.Syntax;
		}
		else
		{
			return null;
		}

		if (binder == null)
		{
			return null;
		}

		BoundNode functionBody = binder.BindFunctionBody(syntax, _diagnostics);
		if (functionBody is BoundFunctionBody body && !functionBody.HasErrors)
		{
			return body.BlockBody;
		}

		Debug.WriteLine($"Function '{function.ToDisplayString()}' is bounded with errors: the MIR and IR processes are invalid when bound node contains any error!");
		return null;
	}

	private void DeclareFunction(FunctionSymbol function)
	{
		FunctionPlan plan = _plans[function];
		if (plan.LlvmFunction.Handle != IntPtr.Zero)
		{
			return;
		}

		LLVMValueRef llvmFunction = _module.AddFunction(GetFunctionName(function), CreateFunctionType(function));
		for (int i = 0; i < function.Parameters.Length; i++)
		{
			llvmFunction.GetParam((uint)i).Name = function.Parameters[i].Name;
		}

		plan.LlvmFunction = llvmFunction;
	}

	private static string GetFunctionName(FunctionSymbol function, bool mangled = false)
	{
		if (mangled)
		{
			return Mangler.Mangle(function);
		}

		return function.ToDisplayString(SymbolFormat.Metadata);
	}

	private void EmitStaticFields(ImmutableArray<LibrarySymbol> libraries)
	{
		foreach (LibrarySymbol library in libraries)
		{
			CollectStaticFields(library.GlobalModule);
		}
	}

	private void CollectStaticFields(ModuleSymbol module)
	{
		foreach (Symbol member in module.GetMembersUnordered())
		{
			switch (member)
			{
				case NamedTypeSymbol namedType:
					foreach (Symbol sym in namedType.GetMembers())
					{
						if (sym is FieldSymbol { IsStatic: true } field)
						{
							string name = Mangler.Mangle(field);
							LLVMTypeRef type = GetLlvmType(field.Type);
							LLVMValueRef global = _module.AddGlobal(type, name);
							global.Initializer = LLVMValueRef.CreateConstNull(type);
							global.Linkage = LLVMLinkage.LLVMInternalLinkage;
						}
					}
					break;
				case ModuleSymbol nestedModule:
					CollectStaticFields(nestedModule);
					break;
			}
		}
	}

	private sealed class FunctionPlan(FunctionSymbol function, ControlFlowGraph? cfg, NirFunction? nir)
	{
		public FunctionSymbol Function { get; } = function;
		public ControlFlowGraph? Cfg { get; } = cfg;
		public NirFunction? Nir { get; } = nir;
		public LLVMValueRef LlvmFunction { get; set; }
	}
}
