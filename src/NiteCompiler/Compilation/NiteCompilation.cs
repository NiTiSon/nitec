using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;

namespace NiteCompiler.Compilation;

public sealed class NiteCompilation
{
	public DiagnosticBag Diagnostics { get; } = [];

	public NiteCompilation(
		string? libraryName,
		IEnumerable<SyntaxTree>? syntaxTrees = null,
		IEnumerable<MetadataReference>? references = null,
		NiteCompilationOptions? options = null)
	{
	}

	private static NiteCompilation Create(
		string? assemblyName,
		NiteCompilationOptions options,
		IEnumerable<SyntaxTree>? syntaxTrees,
		IEnumerable<MetadataReference>? references)
	{
		throw new Exception();
	}
}