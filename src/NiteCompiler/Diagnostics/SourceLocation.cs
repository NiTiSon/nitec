using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

internal sealed class SourceLocation(SyntaxTree tree, TextSpan span) : Location
{
	public SourceLocation(SyntaxNode node) : this(node.Tree, node.Span)
	{
		Debug.Assert(node != null);
		Debug.Assert(node.Tree != null);
	}

	public override SyntaxTree SyntaxTree => tree;
	public override TextSpan? Span => span;
}