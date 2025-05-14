using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
	public Token OperatorToken { get; }
	public ExpressionSyntax Expression { get; }

	public UnaryExpressionSyntax(Token operatorToken, ExpressionSyntax expression)
	{
		OperatorToken = operatorToken;
		Expression = expression;
	}	
	public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return OperatorToken;
		yield return Expression;
	}
}