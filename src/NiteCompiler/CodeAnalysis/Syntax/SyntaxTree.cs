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

	public static SyntaxTree Load(FileInfo file)
	{
		string text = File.ReadAllText(file.FullName);
		StringText sourceText = new(text);
		SyntaxTree syntaxTree = new(sourceText, file.FullName);

		NiteLexer lexer = new(syntaxTree, syntaxTree.Diagnostics);
		NiteParser parser = new(lexer, syntaxTree, syntaxTree.Diagnostics);

		CompilationUnitSyntax unit = parser.Parse();
		foreach (SyntaxNode node in unit.GetNodes(includeThisToken: true))
		{
			node.SyntaxTree = syntaxTree;
		}

		return syntaxTree;
	}
}