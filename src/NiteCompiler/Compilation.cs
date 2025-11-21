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

		foreach (SyntaxTree tree in trees)
		{
			Declarator.DeclarationPass(tree, ModuleManager, Diagnostics);
		}

		CompilationUnitBinder binder = new(this, null);
		foreach (SyntaxTree tree in trees)
		{
			binder.Bind(tree.Root);
		}

		GlobalScope.Diagnostics.DrainInto(Diagnostics);
	}

	public Symbol? GetSymbol(SyntaxNode syntax)
	{
		// TODO: Improve
		IEnumerable<IMemberSymbol> symbols = GlobalScope.ThisLibrary.Modules
			.SelectMany(t => t.Members);
		if (syntax is FunctionDeclarationSyntax)
		{
			return symbols.OfType<SourceFunctionSymbol>().FirstOrDefault(t => t.Syntax == syntax);
		}
		if (syntax is TypeDeclarationSyntax)
		{
			return symbols.OfType<SourceTypeSymbol>().FirstOrDefault(t => t.Syntax == syntax);
		}

		return null;
	}

	public ImmutableArray<Diagnostic> Emit(Stream stream)
	{
		if (!stream.CanWrite) throw new ArgumentException("Stream must be writable", nameof(stream));

		// TODO: Later
		return [];
	}
}