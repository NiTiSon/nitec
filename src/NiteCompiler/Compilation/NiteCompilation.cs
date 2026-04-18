using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Dependencies;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;
using static LLVMSharp.Interop.LLVM;

namespace NiteCompiler.Compilation;

public sealed partial class NiteCompilation
{
	private const string FallbackLibraryName = "unnamed-library";

	internal DeclarationTable Declarations { get; }
	internal MergedModuleDeclaration MergedRoot => Declarations.GetMergedRoot(this);

	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public NiteCompilationOptions Options { get; }
	public DiagnosticBag Diagnostics { get; } = [];
	internal SourceLibrarySymbol SourceLibrary { get; }

	internal BuiltInOperators BuiltInOperators
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, new BuiltInOperators(this), null);
			}

			return field;
		}
	}

	private NiteCompilation(
		string libraryName,
		ImmutableArray<SyntaxTree> syntaxTrees,
		ImmutableArray<Dependency> dependencies,
		NiteCompilationOptions options)
	{
		SyntaxTrees = syntaxTrees;
		Options = options;

		if (Options.IsCoreLibrary && !dependencies.IsEmpty)
		{
			Diagnostics.ReportDependenciesInCoreLibrary();
		}

		Declarations = new(syntaxTrees);

		SourceLibrary = new(this, libraryName);
	}

	public static NiteCompilation Create(
		string libraryName,
		IEnumerable<SyntaxTree>? sourceFiles,
		IEnumerable<Dependency>? dependencies,
		NiteCompilationOptions options)
	{
		ImmutableArray<SyntaxTree> syntaxTrees = sourceFiles?.ToImmutableArray() ?? [];
		ImmutableArray<Dependency> dependenciesArray = dependencies?.ToImmutableArray() ?? [];

		libraryName ??= FallbackLibraryName;
		return new NiteCompilation(libraryName, syntaxTrees, dependenciesArray, options);
	}

	internal int GetSyntaxTreeOrdinal(SyntaxTree tree)
	{
		// TODO: Improve
		int i;
		for (i = 0; i < SyntaxTrees.Length; i++)
		{
			if (tree == SyntaxTrees[i])
			{
				return i;
			}
		}

		throw new KeyNotFoundException($"SyntaxTree is not used within this compilation: '{tree.FilePath ?? "empty-filepath"}'");
	}

	internal bool LookingForSpecialTypes
	{
		get
		{
			if (_lateinitSpecialTypes == null) return true;

			for (int i = 1; i < _lateinitSpecialTypes.Length; i++)
			{
				if (_lateinitSpecialTypes[i] == null) return true;
			}

			return false;
		}
	}

	private TypeSymbol?[]? _lateinitSpecialTypes = null;
	internal void RegisterSpecialType(TypeSymbol type)
	{
		if (_lateinitSpecialTypes == null)
		{
			Interlocked.CompareExchange(ref _lateinitSpecialTypes, new TypeSymbol[(int)SpecialType.Count], null);
		}

		_lateinitSpecialTypes[(int)type.SpecialType] = type;
		Debug.Assert(LookingForSpecialTypes);
	}

	public TypeSymbol? GetSpecialType(SpecialType type)
	{
		Debug.Assert(type != SpecialType.None);

		if (type >= SpecialType.Count)
		{
			throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(SpecialType));
		}

		return _lateinitSpecialTypes?[(int)type];
	}

	public void EmitObjectFile() => throw new NotImplementedException();
	public void EmitAssemblyFile() => throw new NotImplementedException();

	public unsafe void EmitLLVMModule()
	{
		// just a stub herě
		InitializeAllTargets();
		InitializeAllTargetInfos();
		InitializeAllTargetMCs();
		InitializeAllAsmPrinters();

		using LLVMContextRef context = LLVMContextRef.Create();
		using LLVMModuleRef module = LLVMModuleRef.CreateWithName("__ananas");

		LLVMTypeRef int32_t = LLVMTypeRef.Int32;
		LLVMTypeRef addI32_fun_t = LLVMTypeRef.CreateFunction(int32_t, [int32_t, int32_t]);
		LLVMValueRef addI32_fun = module.AddFunction("add", addI32_fun_t);

		using LLVMBuilderRef builder = CreateBuilderInContext(context);
		LLVMBasicBlockRef entry = addI32_fun.AppendBasicBlock("entry");
		builder.PositionAtEnd(entry);
		LLVMValueRef param0 = GetParam(addI32_fun, 0);
		LLVMValueRef param1 = GetParam(addI32_fun, 1);
		param0.Name = "x1";
		param1.Name = "x2";
		LLVMValueRef sum = builder.BuildAdd(param0, param1, "sum");
		builder.BuildRet(sum);

		module.Verify(LLVMVerifierFailureAction.LLVMReturnStatusAction);

		var targetTriple = LLVMTargetRef.DefaultTriple;

		LLVMTargetRef target = LLVMTargetRef.GetTargetFromTriple(targetTriple);
		LLVMTargetMachineRef targetMachine = target.CreateTargetMachine(
			targetTriple, "generic", "", LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault,
			LLVMRelocMode.LLVMRelocDefault, LLVMCodeModel.LLVMCodeModelDefault);
		targetMachine.EmitToFile(module, "add.asm", LLVMCodeGenFileType.LLVMAssemblyFile);
		targetMachine.EmitToFile(module, "add.o", LLVMCodeGenFileType.LLVMObjectFile);
	}

	public void EmitNiteLibrary(Stream stream, out DiagnosticBag? resultDiagnostics)
	{
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
		MetadataLibraryBuilder.Translate(this, stream, diagnostics);

		resultDiagnostics = diagnostics.ToBagAndFree();
	}
}