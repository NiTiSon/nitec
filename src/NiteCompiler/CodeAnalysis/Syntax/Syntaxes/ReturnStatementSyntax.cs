using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ReturnStatementSyntax : StatementSyntax
{
	public Token Return { get; }
	public ExpressionSyntax? Expression { get; }

	public ReturnStatementSyntax(Token returnToken, ExpressionSyntax? expression)
	{
		Return = returnToken;
		Expression = expression;
	}

	public override TextSpan Span
	{
		get
		{
			if (Expression == null)
			{
				return Return.Span;
			}

			return TextSpan.FromBounds(Expression.Span, Expression.Span);
		}
	}

	public override SyntaxKind Kind => SyntaxKind.ReturnStatement;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Return;
		if (Expression != null)
		{
			yield return Expression;
		}
	}
}