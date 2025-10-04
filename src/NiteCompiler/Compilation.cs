using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler;

public sealed class Compilation
{
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	private readonly GlobalSymbolTable _globalSymbolTable;
	public LibrarySymbol Library => _globalSymbolTable.Library;

	public DiagnosticBag Diagnostics { get; }

	public Compilation(string libraryName, params SyntaxTree[] trees)
	{
		SyntaxTrees = [..trees];
		_globalSymbolTable = new(libraryName);
		Diagnostics = [];

		DeclarationPass();

		foreach (ModuleSymbol module in _globalSymbolTable.Modules)
		{
			Console.WriteLine(module.FullName);
			foreach (Symbol symbol in module)
			{
				Console.WriteLine($"\t{symbol}");
			}
		}
	}

	private void DeclarationPass()
	{
		foreach (SyntaxTree tree in SyntaxTrees)
		{
			ModuleSymbol currentModule = Library.GetOrAddModule(null);
			foreach (SyntaxNode topLevelNode in tree.Root.TopLevelNodes)
			{
				if (topLevelNode is ModuleDeclarationSyntax module)
				{
					currentModule = Library.GetOrAddModule(module.Name.GetName());
				}
				else if (topLevelNode is FunctionDeclarationSyntax function)
				{
					FunctionSymbol symbol = new(
						function.Name.GetName(),
						containingSymbol: currentModule,
						function);

					currentModule.AddMember(symbol);
				}
			}
		}
	}

	public void Emit(Stream stream)
	{

	}
}