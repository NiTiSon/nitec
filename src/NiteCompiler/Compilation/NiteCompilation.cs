using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Dependencies;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;
using static LLVMSharp.Interop.LLVM;

namespace NiteCompiler.Compilation;

public sealed class NiteCompilation
{
	private const string FallbackLibraryName = "unnamed-library";

	internal DeclarationTable Declarations { get; }

	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public NiteCompilationOptions Options { get; }
	public DiagnosticBag Diagnostics { get; } = [];
	internal SourceLibrarySymbol SourceLibrary { get; }

	private NiteCompilation(
		string libraryName,
		ImmutableArray<SyntaxTree> syntaxTrees,
		ImmutableArray<Dependency> dependencies,
		NiteCompilationOptions options)
	{
		SyntaxTrees = syntaxTrees;
		Options = options;
		foreach (SyntaxTree tree in syntaxTrees)
		{
			tree.Diagnostics.DrainInto(Diagnostics);
		}

		if (Options.IsCoreLibrary && !dependencies.IsEmpty)
		{
			Diagnostics.ReportDependenciesInCoreLibrary();
		}

		Declarations = new(syntaxTrees);

		SourceLibrary = new(this, Declarations.GetMergedRoot(this), libraryName);
		SourceLibrary.ForceComplete(null);

		_ = 0x3; // breakpoint
	}

	public static NiteCompilation Create(
		string? libraryName,
		IEnumerable<FileInfo>? sourceFiles,
		IEnumerable<Dependency>? dependencies,
		NiteCompilationOptions? options)
	{
		var sources = ImmutableArray.CreateBuilder<SyntaxTree>();
		options ??= NiteCompilationOptions.Default;
		foreach (var sourceFile in sourceFiles ?? [])
		{
			SyntaxTree syntaxTree = SyntaxTree.FromFile(sourceFile, options);
			sources.Add(syntaxTree);
		}

		return Create(libraryName, sources.ToImmutableArray(), dependencies, options);
	}

	private static NiteCompilation Create(
		string? libraryName,
		IEnumerable<SyntaxTree>? sourceFiles,
		IEnumerable<Dependency>? dependencies,
		NiteCompilationOptions options)
	{
		ImmutableArray<SyntaxTree> syntaxTrees = sourceFiles?.ToImmutableArray() ?? [];
		ImmutableArray<Dependency> dependenciesArray = dependencies?.ToImmutableArray() ?? [];

		if (libraryName is null && !syntaxTrees.IsEmpty)
		{
			libraryName = Path.GetFileNameWithoutExtension(syntaxTrees[0].Filename);
		}

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

		throw new KeyNotFoundException($"SyntaxTree is not used within this compilation: '{tree.Filename ?? "empty-filepath"}'");
	}

	private WeakReference<BinderFactory>?[]? _binderFactories;

	internal BinderFactory GetBinderFactory(SyntaxTree syntaxTree)
	{
		return GetBinderFactory(syntaxTree, ref _binderFactories);
	}

	private BinderFactory GetBinderFactory(SyntaxTree syntaxTree, ref WeakReference<BinderFactory>?[]? cachedBinderFactories)
	{
		var treeNum = GetSyntaxTreeOrdinal(syntaxTree);
		WeakReference<BinderFactory>?[]? binderFactories = cachedBinderFactories;
		if (binderFactories == null)
		{
			binderFactories = new WeakReference<BinderFactory>[this.SyntaxTrees.Length];
			binderFactories = Interlocked.CompareExchange(ref cachedBinderFactories, binderFactories, null) ?? binderFactories;
		}

		var previousWeakReference = binderFactories[treeNum];
		if (previousWeakReference != null && previousWeakReference.TryGetTarget(out BinderFactory? previousFactory))
		{
			return previousFactory;
		}

		return AddNewFactory(syntaxTree, ref binderFactories[treeNum]);
	}

	private BinderFactory AddNewFactory(SyntaxTree syntaxTree, [NotNull] ref WeakReference<BinderFactory>? slot)
	{
		var newFactory = new BinderFactory(this, syntaxTree);
		var newWeakReference = new WeakReference<BinderFactory>(newFactory);

		while (true)
		{
			WeakReference<BinderFactory>? previousWeakReference = slot;
			if (previousWeakReference != null && previousWeakReference.TryGetTarget(out BinderFactory? previousFactory))
			{
				Debug.Assert(slot != null);
				return previousFactory;
			}

			if (Interlocked.CompareExchange(ref slot!, newWeakReference, previousWeakReference) == previousWeakReference)
			{
				return newFactory;
			}
		}
	}

	internal Binder GetBinder(SyntaxNode node)
	{
		return GetBinderFactory(node.Tree).GetBinder(node);
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
		if (type == SpecialType.None) return null;

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

		var targetTriple = "x86_64-unknown-windows";

		LLVMTargetRef target = LLVMTargetRef.GetTargetFromTriple(targetTriple);
		LLVMTargetMachineRef targetMachine = target.CreateTargetMachine(
			targetTriple, "generic", "", LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault,
			LLVMRelocMode.LLVMRelocDefault, LLVMCodeModel.LLVMCodeModelDefault);
		targetMachine.EmitToFile(module, "add.asm", LLVMCodeGenFileType.LLVMAssemblyFile);
		targetMachine.EmitToFile(module, "add.o", LLVMCodeGenFileType.LLVMObjectFile);
	}

	public void EmitNiteLibrary(Stream stream)
	{
		MetadataLibraryBuilder.Translate(this, stream);
	}
}