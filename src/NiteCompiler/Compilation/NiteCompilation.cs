using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Dependencies;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Compilation;

public sealed class NiteCompilation
{
	private const string FallbackLibraryName = "unnamed-library";

	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public NiteCompilationOptions Options { get; }
	public DiagnosticBag Diagnostics { get; } = [];

	private NiteCompilation(
		string? libraryName,
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
	}

	public static NiteCompilation Create(
		string? libraryName,
		IEnumerable<FileInfo>? sourceFiles,
		IEnumerable<Dependency>? dependencies,
		NiteCompilationOptions? options)
	{
		var sources = ImmutableArray.CreateBuilder<SyntaxTree>();
		foreach (var sourceFile in sourceFiles ?? [])
		{
			SyntaxTree syntaxTree = SyntaxTree.Load(sourceFile);
			sources.Add(syntaxTree);
		}

		return Create(libraryName, sources.ToImmutableArray(), dependencies, options);
	}

	private static NiteCompilation Create(
		string? libraryName,
		IEnumerable<SyntaxTree>? sourceFiles,
		IEnumerable<Dependency>? dependencies,
		NiteCompilationOptions? options)
	{
		ImmutableArray<SyntaxTree> syntaxTrees = sourceFiles?.ToImmutableArray() ?? [];
		ImmutableArray<Dependency> dependenciesArray = dependencies?.ToImmutableArray() ?? [];

		if (libraryName is null && !syntaxTrees.IsEmpty)
		{
			libraryName = Path.GetFileNameWithoutExtension(syntaxTrees[0].Filename);
		}

		libraryName ??= FallbackLibraryName;
		options ??= NiteCompilationOptions.Default;
		return new NiteCompilation(libraryName, syntaxTrees, dependenciesArray, options);
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