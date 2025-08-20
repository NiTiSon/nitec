using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Expressions;

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Left { get; }
	public Token Operator { get; }
	public ExpressionSyntax Right { get; }

	public BinaryExpressionSyntax(ExpressionSyntax left, Token @operator, ExpressionSyntax right)
	{
		Left = left;
		Operator = @operator;
		Right = right;
	}

	public override TextSpan Span => TextSpan.FromBounds(Left.Span.Start, Right.Span.End);
	public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return Operator;
		yield return Right;
	}
}