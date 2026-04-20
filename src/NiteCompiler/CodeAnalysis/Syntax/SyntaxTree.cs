using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
	public SourceText Text { get; }
	public CompilationUnitSyntax Root { get; private set; } = null!;
	private DiagnosticBag Diagnostics { get; } = [];
	public string? FilePath { get; }

	private SyntaxTree(SourceText text, string? filePath)
	{
		Text = text;
		FilePath = filePath;
	}

	public static SyntaxTree ParseText(string text, string? path, NiteCompilationOptions options,
		CancellationToken cancellationToken = default)
	{
		return ParseText(SourceText.FromText(text), path, options, cancellationToken);
	}

	public static SyntaxTree ParseText(SourceText text, string? path, NiteCompilationOptions options, CancellationToken cancellationToken = default)
	{
		SyntaxTree syntaxTree = new(text, path);

		NiteLexer lexer = new(syntaxTree, options, syntaxTree.Diagnostics);
		NiteParser parser = new(lexer, syntaxTree, syntaxTree.Diagnostics);

		CompilationUnitSyntax root = parser.Parse();
		syntaxTree.Root = root;

		return syntaxTree;
	}

	private Dictionary<SyntaxNode, SyntaxNode?>? _parentMap;
	internal SyntaxNode? GetParent(SyntaxNode syntaxNode)
	{
		Debug.Assert(syntaxNode != null);
		Debug.Assert(syntaxNode.Tree == this);

		if (_parentMap == null)
		{
			Interlocked.CompareExchange(ref _parentMap, new(), null);
		}

		lock (_parentMap)
		{
			ref SyntaxNode? parent = ref CollectionsMarshal.GetValueRefOrAddDefault(_parentMap, syntaxNode, out bool exists);

			if (exists)
			{
				return parent;
			}
			else
			{
				parent = GetParentSlow(syntaxNode);
				return parent;
			}
		}
	}

	private SyntaxNode? GetParentSlow(SyntaxNode syntaxNode)
	{
		if (ReferenceEquals(Root, syntaxNode))
			return null;

		SyntaxNode current = Root;

		while (true)
		{
			bool descended = false;

			foreach (SyntaxNode child in current.GetChildren())
			{
				if (ReferenceEquals(child, syntaxNode))
					return current;

				if (child.Span.Contains(syntaxNode.Span))
				{
					current = child;
					descended = true;
					break;
				}
			}

			if (!descended)
				return null;
		}
	}

	public IEnumerable<Diagnostic> GetDiagnostics(CancellationToken cancellationToken = default)
	{
		return Diagnostics;
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