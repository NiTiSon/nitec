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

	public TextLocation Location => new TextLocation(SyntaxTree.Text, Span);

	public abstract SyntaxKind Kind { get; }
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

	public void WriteTo(TextWriter writer)
	{
		PrettyPrint(writer, this);
	}

	private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
	{
		bool isToConsole = writer == Console.Out;
		SyntaxToken? token = node as SyntaxToken;

		if (token != null)
		{
			foreach (var trivia in token.LeadingTrivia)
			{
				if (isToConsole)
					Console.ForegroundColor = ConsoleColor.DarkGray;

				writer.Write(indent);
				writer.Write("├──");

				if (isToConsole)
					Console.ForegroundColor = ConsoleColor.DarkGreen;

				writer.WriteLine($"L: {trivia.Kind}");
			}
		}

		var hasTrailingTrivia = token != null && token.TrailingTrivia.Any();
		var tokenMarker = !hasTrailingTrivia && isLast ? "└──" : "├──";

		if (isToConsole)
			Console.ForegroundColor = ConsoleColor.DarkGray;

		writer.Write(indent);
		writer.Write(tokenMarker);

		if (isToConsole)
			Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;

		writer.Write(node.Kind);

		if (token != null && token.Location.Text.Length != 0)
		{
			writer.Write(": ");
			writer.Write(token.Text.Value);
		}

		if (isToConsole)
			Console.ResetColor();

		writer.WriteLine();

		if (token != null)
		{
			foreach (var trivia in token.TrailingTrivia)
			{
				var isLastTrailingTrivia = trivia == token.TrailingTrivia.Last();
				var triviaMarker = isLast && isLastTrailingTrivia ? "└──" : "├──";

				if (isToConsole)
					Console.ForegroundColor = ConsoleColor.DarkGray;

				writer.Write(indent);
				writer.Write(triviaMarker);

				if (isToConsole)
					Console.ForegroundColor = ConsoleColor.DarkGreen;

				writer.WriteLine($"T: {trivia.Kind}");
			}
		}

		indent += isLast ? "   " : "│  ";

		var lastChild = node.GetChildren().LastOrDefault();

		foreach (var child in node.GetChildren())
			PrettyPrint(writer, child, indent, child == lastChild);
	}
}
