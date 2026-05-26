using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class CastExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Left { get; }
	public Token AsToken { get; }
	public ExpressionSyntax TypeExpression { get; }

	public CastExpressionSyntax(SyntaxTree tree, ExpressionSyntax left, Token asToken, ExpressionSyntax typeExpression)
		: base(tree)
	{
		Left = left;
		AsToken = asToken;
		TypeExpression = typeExpression;
	}

	public override NodeKind Kind => NodeKind.CastExpression;
	public override TextSpan Span => TextSpan.FromBounds(Left.Span.Start, TypeExpression.Span.End);

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitCastExpression(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitCastExpression(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return AsToken;
		yield return TypeExpression;
	}
}
