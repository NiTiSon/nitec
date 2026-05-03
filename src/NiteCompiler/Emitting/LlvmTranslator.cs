using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.Emitting;

internal sealed partial class LlvmTranslator
{
	private readonly LLVMContextRef _context;
	private readonly LLVMModuleRef _module;
	private readonly LLVMBuilderRef _builder;
	private readonly BindingDiagnosticBag _diagnostics;
	private readonly Dictionary<FunctionSymbol, FunctionPlan> _plans = new();

	private LlvmTranslator(string moduleName, BindingDiagnosticBag diagnostics)
	{
		_context = LLVMContextRef.Global;
		_module = _context.CreateModuleWithName(moduleName);
		_builder = _context.CreateBuilder();
		_diagnostics = diagnostics;
	}

	public static LLVMModuleRef Translate(ImmutableArray<LibrarySymbol> libraries, FunctionSymbol? entryPoint,
		BindingDiagnosticBag diagnostics)
	{
		string moduleName = libraries.IsDefaultOrEmpty ? "__nite" : libraries[0].Name;
		var translator = new LlvmTranslator(moduleName, diagnostics);
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

			if (!_plans.TryGetValue(function, out FunctionPlan? plan) || plan.Ssa == null)
			{
				continue;
			}

			foreach (FunctionSymbol callee in EnumerateCalledFunctions(plan.Ssa))
			{
				if (discovered.Add(callee))
				{
					worklist.Enqueue(callee);
				}
			}
		}
	}

	private static IEnumerable<FunctionSymbol> EnumerateCalledFunctions(SsaFunction ssa)
	{
		foreach (SsaBlock block in ssa.Blocks.Values)
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
			_plans[function] = new FunctionPlan(function, cfg: null, ssa: null);
			return;
		}

		BoundBlock? body = BindFunctionBody(function);
		if (body == null)
		{
			_plans[function] = new FunctionPlan(function, cfg: null, ssa: null);
			return;
		}

		ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(function, body, _diagnostics);
		SsaFunction ssa = SsaBuilder.Build(cfg, function);
		_plans[function] = new FunctionPlan(function, cfg, ssa);
	}

	private BoundBlock? BindFunctionBody(FunctionSymbol function)
	{
		if (function is not SourceFunctionSymbol sourceFunction)
		{
			return null;
		}

		Binder? binder = sourceFunction.TryGetBodyBinder();
		if (binder == null)
		{
			return null;
		}

		BoundNode functionBody = binder.BindFunctionBody(sourceFunction.Syntax, _diagnostics);
		if (functionBody is BoundFunctionBody body && !functionBody.HasErrors)
		{
			return body.BlockBody;
		}

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

	private sealed class FunctionPlan(FunctionSymbol function, ControlFlowGraph? cfg, SsaFunction? ssa)
	{
		public FunctionSymbol Function { get; } = function;
		public ControlFlowGraph? Cfg { get; } = cfg;
		public SsaFunction? Ssa { get; } = ssa;
		public LLVMValueRef LlvmFunction { get; set; }
	}
}
