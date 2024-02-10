using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class ExpressionStatementSyntax : StatementSyntax
{
	public ExpressionStatementSyntax(SyntaxTree tree, ExpressionSyntax expression) : base(tree)
	{
		Expression = expression;
	}

	public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

	public ExpressionSyntax Expression { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Expression;
	}
}