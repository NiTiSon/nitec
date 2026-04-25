using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class AssignmentExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Left { get; }
	public Token OperatorToken { get; }
	public ExpressionSyntax Right { get; }
	public override TextSpan Span => TextSpan.FromBounds(Left.Span.Start, Right.Span.End);
	public override NodeKind Kind { get; }

	internal AssignmentExpressionSyntax(SyntaxTree tree, ExpressionSyntax lhs, Token operatorToken,
		ExpressionSyntax rhs, NodeKind operatorExpressionKind) : base(tree)
	{
		Left = lhs;
		OperatorToken = operatorToken;
		Right = rhs;
		Kind = operatorExpressionKind;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAssignmentExpression(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitAssignmentExpression(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return OperatorToken;
		yield return Right;
	}
}