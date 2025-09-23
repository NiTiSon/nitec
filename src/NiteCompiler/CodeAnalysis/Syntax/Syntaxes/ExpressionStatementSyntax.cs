using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ExpressionStatementSyntax : StatementSyntax
{
	public ExpressionSyntax Expression { get; }

	public ExpressionStatementSyntax(ExpressionSyntax expression)
	{
		Expression = expression;
	}

	public override TextSpan Span => Expression.Span;
	public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Expression;
	}
}