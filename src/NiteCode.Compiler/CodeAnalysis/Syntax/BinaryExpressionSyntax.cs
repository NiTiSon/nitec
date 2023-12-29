using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class BinaryExpressionSyntax(SyntaxTree tree, ExpressionSyntax left, SyntaxToken @operator, ExpressionSyntax right) : ExpressionSyntax(tree)
{
	public ExpressionSyntax Left { get; } = left;
	public SyntaxToken OperatorToken { get; } = @operator;
	public ExpressionSyntax Right { get; } = right;

	public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return OperatorToken;
		yield return Right;
	}
}