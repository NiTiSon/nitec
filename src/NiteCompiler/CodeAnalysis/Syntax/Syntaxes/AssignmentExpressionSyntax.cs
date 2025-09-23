using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class AssignmentExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Left { get; }

	public Token OperatorToken { get; }

	public ExpressionSyntax Right { get; }

	public override SyntaxKind Kind { get; }

	public AssignmentExpressionSyntax(ExpressionSyntax left, Token operatorToken, ExpressionSyntax right, SyntaxKind assignmentKind)
	{
		Left = left;
		OperatorToken = operatorToken;
		Right = right;
		Kind = assignmentKind;
	}

	public override TextSpan Span => TextSpan.FromBounds(Left.Span, Right.Span);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return OperatorToken;
		yield return Right;
	}
}