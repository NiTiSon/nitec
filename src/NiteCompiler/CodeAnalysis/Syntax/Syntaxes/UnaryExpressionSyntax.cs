using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public class UnaryExpressionSyntax : ExpressionSyntax
{
	public Token Operator { get; }
	public ExpressionSyntax Expression { get; }
	public override TextSpan Span => TextSpan.FromBounds(Operator.Span.Start, Expression.Span.End);
	public override NodeKind Kind { get; }

	public UnaryExpressionSyntax(SyntaxTree tree, Token @operator, ExpressionSyntax expression, NodeKind operatorKind) : base(tree)
	{
		Operator = @operator;
		Expression = expression;
		Kind = operatorKind;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Operator;
		yield return Expression;
	}
}