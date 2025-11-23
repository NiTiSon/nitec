using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class DeclarationPass
{
	private readonly Dictionary<SyntaxNode, Symbol> _declared = [];

	public void Declare(params ReadOnlySpan<SyntaxTree> trees)
	{
		foreach (var tree in trees)
		{
			DeclareCompilationUnit(tree.Root);
		}
	}

	private void DeclareCompilationUnit(CompilationUnitSyntax unit)
	{

	}

	public Symbol? GetDeclaredSymbol(SyntaxNode node)
	{
		TryGetDeclaredSymbol(node, out var symbol);
		return symbol;
	}

	public bool TryGetDeclaredSymbol(SyntaxNode syntax, [NotNullWhen(true)] out Symbol? symbol)
	{
		return _declared.TryGetValue(syntax, out symbol);
	}
}