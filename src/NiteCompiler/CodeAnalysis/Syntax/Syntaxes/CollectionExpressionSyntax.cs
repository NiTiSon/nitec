using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class CollectionExpressionSyntax : ExpressionSyntax
{
	public Token OpenBracket { get; }
	public SyntaxList<ExpressionSyntax> Elements { get; }
	public Token CloseBracket { get; }

	public override NodeKind Kind => NodeKind.CollectionExpression;
	public override TextSpan Span => TextSpan.FromBounds(OpenBracket.Span, CloseBracket.Span);

	internal CollectionExpressionSyntax(SyntaxTree tree, Token openBracket, SyntaxList<ExpressionSyntax> elements, Token closeBracket) : base(tree)
	{
		OpenBracket = openBracket;
		Elements = elements;
		CloseBracket = closeBracket;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBracket;
		yield return Elements;
		yield return CloseBracket;
	}

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitCollectionExpression(this);

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitCollectionExpression(this);
}
