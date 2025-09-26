using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Left { get; }
	public Token Operator { get; }
	public ExpressionSyntax Right { get; }

	public override SyntaxKind Kind { get; }

	public BinaryExpressionSyntax(ExpressionSyntax left, Token @operator, ExpressionSyntax right, SyntaxKind kind)
	{
		Left = left;
		Operator = @operator;
		Right = right;
		Kind = kind;
	}

	public override TextSpan Span => TextSpan.FromBounds(Left.Span.Start, Right.Span.End);
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return Operator;
		yield return Right;
	}
}