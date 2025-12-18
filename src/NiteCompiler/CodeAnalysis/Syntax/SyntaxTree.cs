using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NiteCompiler.CodeAnalysis.Text;
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

	public static SyntaxTree FromFile(FileInfo file)
	{
		string text = File.ReadAllText(file.FullName);
		return Create(text, file.FullName);
	}

	public static SyntaxTree FromText(string text, string? filename)
	{
		return Create(text, filename);
	}

	public static SyntaxTree Create(string text, string? filename)
	{
		StringText sourceText = new(text);
		SyntaxTree syntaxTree = new(sourceText, filename);

		NiteLexer lexer = new(syntaxTree, syntaxTree.Diagnostics);
		NiteParser parser = new(lexer, syntaxTree, syntaxTree.Diagnostics);

		CompilationUnitSyntax unit = parser.Parse();
		syntaxTree.Root = unit;
		foreach (SyntaxNode node in unit.GetNodes(includeThisToken: true))
		{
			node.SyntaxTree = syntaxTree;
		}

		return syntaxTree;
	}
}