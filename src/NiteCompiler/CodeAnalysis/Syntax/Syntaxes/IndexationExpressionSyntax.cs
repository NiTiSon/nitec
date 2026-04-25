using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class IndexationExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Expression { get; }
	public BracketedArgumentListSyntax ArgumentList { get; }

	public override NodeKind Kind => NodeKind.InvocationExpression;
	public override TextSpan Span => TextSpan.FromBounds(Expression.Span, ArgumentList.Span);

	internal IndexationExpressionSyntax(SyntaxTree tree, ExpressionSyntax invoked, BracketedArgumentListSyntax argumentList) : base(tree)
	{
		Expression = invoked;
		ArgumentList = argumentList;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Expression;
		yield return ArgumentList;
	}

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitIndexationExpression(this);

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitIndexationExpression(this);
}