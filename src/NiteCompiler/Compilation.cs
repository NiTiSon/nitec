using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;

namespace NiteCompiler;

public sealed class Compilation
{
	internal GlobalScope GlobalScope { get; }
	public ModuleManager ModuleManager { get; }
	public NlibLibrary[] Dependencies { get; }
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public DiagnosticBag Diagnostics { get; }

	public Compilation(string libraryName, NlibLibrary[] dependencies, SyntaxTree[] trees)
	{
		Dependencies = dependencies;
		SyntaxTrees = [..trees];
		GlobalScope = new(libraryName, dependencies);
		ModuleManager = new(GlobalScope);
		Diagnostics = [];

		CompilationBinder rootBinder = new(this);
		foreach (SyntaxTree tree in trees)
		{
			Declarator.DeclarationPass(tree, ModuleManager, Diagnostics);
		}

		rootBinder.Bind();

		rootBinder.Diagnostics.DrainInto(Diagnostics);
		GlobalScope.Diagnostics.DrainInto(Diagnostics);
	}

	public void Emit(Stream stream)
	{
		// TODO: Later
	}
}