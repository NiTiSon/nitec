using System.Threading;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal sealed class SourceSyntaxReference : SyntaxReference
{
	public SyntaxNode Node { get; }
	public override SyntaxTree SyntaxTree => Node.Tree;
	public override TextSpan Span => Node.Span;

	public SourceSyntaxReference(SyntaxNode node)
	{
		Node = node;
	}

	public override SyntaxNode GetSyntax(CancellationToken cancellationToken = default)
	{
		return Node;
	}
}