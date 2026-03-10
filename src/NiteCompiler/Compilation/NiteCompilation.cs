using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Text;
using CommunityToolkit.Diagnostics;
using LLVMSharp;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Dependencies;
using NiteCompiler.Diagnostics;
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

	public unsafe void EmitLLVMModule()
	{
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

	public void WriteNiTiSLibrary(Stream stream)
	{
		Guard.CanWrite(stream);
		using ZipArchive file = new(stream, ZipArchiveMode.Create, leaveOpen: true);
		ZipArchiveEntry libInfo = file.CreateEntry("lib-info.yml", CompressionLevel.NoCompression);

		using StreamWriter libInfoWriter = new(libInfo.Open(), Encoding.Default, leaveOpen: false);
		libInfoWriter.Write("Hello World");
	}
}