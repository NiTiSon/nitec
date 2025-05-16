using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ParenthesizeExpressionSyntax : ExpressionSyntax
{
	public Token OpenParenToken { get; }
	public ExpressionSyntax Expression { get; }
	public Token CloseParenToken { get; }

	public ParenthesizeExpressionSyntax(Token openParenToken, ExpressionSyntax expression, Token closeParenToken)
	{
		OpenParenToken = openParenToken;
		Expression = expression;
		CloseParenToken = closeParenToken;
	}

	public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return OpenParenToken;
		yield return Expression;
		yield return CloseParenToken;
	}
}