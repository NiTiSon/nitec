using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
{
	public Token OpenParenToken { get; }
	public ExpressionSyntax Expression { get; }
	public Token CloseParenToken { get; }

	public override TextSpan Span => TextSpan.FromBounds(OpenParenToken.Span.Start, CloseParenToken.Span.End);
	public override NodeKind Kind => NodeKind.ParenthesizedExpression;

	public ParenthesizedExpressionSyntax(SyntaxTree tree, Token openParen, ExpressionSyntax expression, Token closeParen) : base(tree)
	{
		OpenParenToken = openParen;
		Expression = expression;
		CloseParenToken = closeParen;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenParenToken;
		yield return Expression;
		yield return CloseParenToken;
	}
}