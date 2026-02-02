using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
	public SyntaxTree Tree { get; }
	public abstract TextSpan Span { get; }
	public abstract NodeKind Kind { get; }

	public Location Location
	{
		get
		{
			field ??= Location.Create(Tree, Span);
			return field;
		}
	}

	private protected SyntaxNode(SyntaxTree tree)
	{
		Tree = tree;
	}

	public bool IsBefore(SyntaxNode afterNode)
	{
		return Span.End == afterNode.Span.Start;
	}

	public override string ToString()
	{
		return $"NodeKind = {Kind}";
	}

	public abstract IEnumerable<SyntaxNode> GetChildren();
}