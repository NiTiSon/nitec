using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Left { get; }
	public Token OperatorToken { get; }
	public ExpressionSyntax Right { get; }

	public BinaryExpressionSyntax(ExpressionSyntax left, Token operatorToken, ExpressionSyntax right)
	{
		Left = left;
		OperatorToken = operatorToken;
		Right = right;
	}
	
	public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return Left;
		yield return OperatorToken;
		yield return Right;
	}
}