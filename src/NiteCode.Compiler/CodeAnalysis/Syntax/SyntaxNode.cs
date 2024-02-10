using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
	private protected SyntaxNode(SyntaxTree tree)
	{
		SyntaxTree = tree;
	}

	public SyntaxTree SyntaxTree { get; }
	public abstract SyntaxKind Kind { get; }

	public TextLocation Location => new(SyntaxTree.Text, Span);

	public abstract IEnumerable<SyntaxNode> GetChildren();

	public virtual TextSpan Span
	{
		get
		{
			TextSpan first = GetChildren().First().Span;
			TextSpan last = GetChildren().Last().Span;
			return TextSpan.FromBounds(first.Position, last.EndPosition);
		}
	}

	public virtual TextSpan FullSpan
	{
		get
		{
			TextSpan first = GetChildren().First().FullSpan;
			TextSpan last = GetChildren().Last().FullSpan;
			return TextSpan.FromBounds(first.Position, last.EndPosition);
		}
	}

	internal void PrettyPrint(TextWriter tw, string indent = "", bool isLast = true)
	{
		// ├── └── │
		bool isConsole = tw == Console.Out;
		string marker = isLast ? "└──" : "├──";

		tw.Write(indent);
		tw.Write(marker);

		if (isConsole)
			Console.ForegroundColor
				= this is SyntaxToken ? ConsoleColor.Cyan
				: this is ExpressionSyntax ? ConsoleColor.Green
				: ConsoleColor.White;

		tw.Write(Kind);

		if (isConsole)
			Console.ResetColor();

		if (this is SyntaxToken token && !string.IsNullOrEmpty(token.Text.Value))
		{
			Console.Write(": ");
			Console.Write(token.Text);
		}

		Console.WriteLine();

		indent += isLast ? "   " : "│  ";

		SyntaxNode[] children = GetChildren().ToArray();
		for (int i = 0; i < children.Length; i++)
		{
			children[i].PrettyPrint(tw, indent, isLast: i == children.Length - 1);
		}
	}
}