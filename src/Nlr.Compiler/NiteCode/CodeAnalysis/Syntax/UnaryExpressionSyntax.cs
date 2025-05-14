using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
	public Token OperatorToken { get; }
	public ExpressionSyntax Right { get; }

	public UnaryExpressionSyntax(Token operatorToken, ExpressionSyntax right)
	{
		OperatorToken = operatorToken;
		Right = right;
	}	
	public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		throw new System.NotImplementedException();
	}
}