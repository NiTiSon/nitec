using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
	public Token Operator { get; }
	public ExpressionSyntax Expression { get; }

	public override SyntaxKind Kind { get; }

	public UnaryExpressionSyntax(Token @operator, ExpressionSyntax expression, SyntaxKind kind)
	{
		Operator = @operator;
		Expression = expression;
		Kind = kind;
	}

	public override TextSpan Span => TextSpan.FromBounds(Operator.Span, Expression.Span);
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Operator;
		yield return Expression;
	}
}