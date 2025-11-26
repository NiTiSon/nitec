using System;
using System.Collections.Immutable;
using System.IO;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;

namespace NiteCompiler;

public sealed class Compilation
{
	public ImmutableArray<NlibLibrary> Dependencies { get; }
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	// private DeclarationPass _declarationPass;
	// private SpecialTypeResolvePass  _specialTypeResolvePass;
	public DiagnosticBag Diagnostics { get; }

	public Compilation(string libraryName, bool buildCoreLib, NlibLibrary[] dependencies, SyntaxTree[] trees)
	{
		Dependencies = [..dependencies];
		SyntaxTrees = [..trees];
		Diagnostics = [];
		if (buildCoreLib && dependencies.Length > 0)
		{
			// Core library is not allowed to have any dependencies
			Diagnostics.ReportDependenciesInCoreLibrary();
		}

		CompilationUnitBinder binder = new(this, null);
		foreach (SyntaxTree tree in trees)
		{
			BoundNode node = binder.Bind(tree.Root);
		}
	}

	public ImmutableArray<Diagnostic> Emit(Stream stream)
	{
		if (!stream.CanWrite) throw new ArgumentException("Stream must be writable", nameof(stream));

		// TODO: Later
		return [];
	}
}