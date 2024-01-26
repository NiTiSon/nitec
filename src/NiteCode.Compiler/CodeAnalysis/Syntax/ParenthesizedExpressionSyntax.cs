using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
{
	public ParenthesizedExpressionSyntax(SyntaxTree tree, SyntaxToken left, ExpressionSyntax expression, SyntaxToken right) : base(tree)
	{
		Left = left;
		Expression = expression;
		Right = right;
	}
	public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

	public SyntaxToken Left { get; }
	public ExpressionSyntax Expression { get; }
	public SyntaxToken Right { get; }


	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return Expression;
		yield return Right;
	}
}