using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
	public UnaryExpressionSyntax(SyntaxTree tree, SyntaxKind kind, SyntaxToken operationToken, ExpressionSyntax expression) : base(tree)
	{
		Expression = expression;
		Operator = operationToken;
		Kind = kind;
	}

	public SyntaxToken Operator { get; }
	public ExpressionSyntax Expression { get; }

	public override SyntaxKind Kind { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Operator;
		yield return Expression;
	}
}
