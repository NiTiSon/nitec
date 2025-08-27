using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax.Directives;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
	public readonly ImmutableArray<ISyntaxTreeTopLevelMember> Members;

	internal SyntaxTree(ImmutableArray<ISyntaxTreeTopLevelMember> members)
	{
		Members = members;
	}

	internal static void PrintTree(SyntaxNode node, string indent, bool isLast)
	{
		Console.Write(indent);
		Console.Write(isLast ? "└─" : "├─");

		Console.ForegroundColor = node is Token ? ConsoleColor.Green : ConsoleColor.Blue;
		Console.WriteLine(node);
		Console.ResetColor();

		indent += isLast ? "  " : "│ ";

		var children = node.GetChildren().ToArray();
		for (int i = 0; i < children.Length; i++)
		{
			PrintTree(children[i], indent, i == children.Length - 1);
		}
	}
}