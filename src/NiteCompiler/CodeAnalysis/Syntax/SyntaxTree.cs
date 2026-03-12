using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
	public SourceText Text { get; }
	public CompilationUnitSyntax Root { get; private set; } = null!;
	public DiagnosticBag Diagnostics { get; } = [];
	public string? Filename { get; }

	private SyntaxTree(SourceText text, string? filename)
	{
		Text = text;
		Filename = filename;
	}

	public static SyntaxTree FromFile(FileInfo file, NiteCompilationOptions options)
	{
		string text = File.ReadAllText(file.FullName);
		return Create(text, options, file.FullName);
	}

	public static SyntaxTree FromText(string text, NiteCompilationOptions options, string? filename)
	{
		return Create(text, options, filename);
	}

	public static SyntaxTree Create(string text, NiteCompilationOptions options, string? filename)
	{
		StringText sourceText = new(text);
		SyntaxTree syntaxTree = new(sourceText, filename);

		NiteLexer lexer = new(syntaxTree, options, syntaxTree.Diagnostics);
		NiteParser parser = new(lexer, syntaxTree, syntaxTree.Diagnostics);

		CompilationUnitSyntax unit = parser.Parse();
		syntaxTree.Root = unit;

		return syntaxTree;
	}

	public void Emit(TextWriter writer)
	{
		PrintTree(writer, this);
	}

	private static void PrintTree(TextWriter tw, SyntaxTree tree, string indent = "", bool isLast = true)
	{
		if (tree.Root.Items.Count == 0) return;

		SyntaxNode lastChild = tree.Root.Items[^1];

		foreach (ItemSyntax child in tree.Root.Items)
			PrintNode(tw, child, indent, child == lastChild);
	}

	private static void PrintNode(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
	{
		string tokenMarker = isLast ? "└──" : "├──";

		writer.Write(indent);
		writer.Write(tokenMarker);
		writer.WriteLine(node);

		indent += isLast ? "   " : "│  ";

		SyntaxNode? lastChild = node.GetChildren().LastOrDefault();

		foreach (SyntaxNode child in node.GetChildren())
			PrintNode(writer, child, indent: indent, isLast: child == lastChild);
	}
}