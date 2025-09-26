using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
{
	public Token OpenParenthesisToken { get; }
	public ExpressionSyntax Expression { get; }
	public Token CloseParenthesisToken { get; }

	public ParenthesizedExpressionSyntax(Token leftParen, ExpressionSyntax expression, Token rightParen)
	{
		OpenParenthesisToken = leftParen;
		Expression = expression;
		CloseParenthesisToken = rightParen;
	}

	public override TextSpan Span => TextSpan.FromBounds(OpenParenthesisToken.Span, CloseParenthesisToken.Span);
	public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenParenthesisToken;
		yield return Expression;
		yield return CloseParenthesisToken;
	}
}