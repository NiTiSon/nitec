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
	public CompilationUnitSyntax Root { get; }
	public ImmutableArray<Diagnostic> Diagnostics { get; }

	private SyntaxTree(SourceText text, CompilationUnitSyntax root, ImmutableArray<Diagnostic> diagnostics)
	{
		Text = text;
		Root = root;
		Diagnostics = diagnostics;
	}

	public static SyntaxTree Load(FileInfo file)
	{
		DiagnosticBag diagnostics = new();
		string text = File.ReadAllText(file.FullName);
		StringText sourceText = new(text, file.FullName);

		NiteLexer lexer = new(sourceText, diagnostics);
		NiteParser parser = new(lexer, sourceText, diagnostics);

		CompilationUnitSyntax unit = parser.Parse();

		SyntaxTree tree = new(sourceText, unit, [..diagnostics]);
		foreach (SyntaxNode node in unit.GetNodes(includeThisToken: true))
		{
			node.SyntaxTree = tree;
		}

		return tree;
	}
}