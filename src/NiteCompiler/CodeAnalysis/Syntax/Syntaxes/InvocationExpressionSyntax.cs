using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class InvocationExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Expression { get; }
	public ArgumentListSyntax ArgumentList { get; }

	public override NodeKind Kind => NodeKind.InvocationExpression;
	public override TextSpan Span => TextSpan.FromBounds(Expression.Span, ArgumentList.Span);

	internal InvocationExpressionSyntax(SyntaxTree tree, ExpressionSyntax invoked, ArgumentListSyntax argumentList) : base(tree)
	{
		Expression = invoked;
		ArgumentList = argumentList;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Expression;
		yield return ArgumentList;
	}

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitInvocationExpression(this);

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitInvocationExpression(this);
}