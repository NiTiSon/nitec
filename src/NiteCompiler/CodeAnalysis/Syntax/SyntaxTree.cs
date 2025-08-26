using System;
using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax.Directives;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
	public readonly ImmutableArray<UseDirectiveSyntax> Usages;

	internal SyntaxTree(ImmutableArray<UseDirectiveSyntax> usages)
	{
		Usages = usages;
	}

	internal static void PrintTree(SyntaxNode node, string indent, bool isLast)
	{
		Console.Write(indent);
		Console.Write(isLast ? "└─" : "├─");
		Console.WriteLine(node);

		indent += isLast ? "  " : "│ ";

		var children = node.GetChildren().ToList();
		for (int i = 0; i < children.Count; i++)
		{
			PrintTree(children[i], indent, i == children.Count - 1);
		}
	}
}