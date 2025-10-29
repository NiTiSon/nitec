using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;

namespace NiteCompiler;

public sealed class Compilation
{
	private readonly GlobalScope _globalScope;
	private readonly ModuleManager _moduleManager;
	public NlibLibrary[] Dependencies { get; }
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public DiagnosticBag Diagnostics { get; }

	public Compilation(string libraryName, NlibLibrary[] dependencies, SyntaxTree[] trees)
	{
		Dependencies = dependencies;
		SyntaxTrees = [..trees];
		_globalScope = new(libraryName);
		_moduleManager = new(_globalScope);
		Diagnostics = [];

		foreach (SyntaxTree tree in trees)
		{
			Declarator.DeclarationPass(tree, _globalScope, _moduleManager, Diagnostics);
		}

		_globalScope.Diagnostics.DrainInto(Diagnostics);
	}

	public void Emit(Stream stream)
	{
	}
}