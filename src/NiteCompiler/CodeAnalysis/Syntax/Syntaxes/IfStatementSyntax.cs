using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class IfStatementSyntax : StatementSyntax
{
	public Token IfToken { get; }
	public ExpressionSyntax Condition { get; }
	public StatementSyntax ThenStatement { get; }
	public ElseClauseSyntax? ElseClause { get; }
	public override TextSpan Span => TextSpan.FromBounds(IfToken.Span, ElseClause?.Span ?? ThenStatement.Span);
	public override NodeKind Kind => NodeKind.IfStatement;

	internal IfStatementSyntax(SyntaxTree tree, Token @if, ExpressionSyntax condition, StatementSyntax thenStatement,
		ElseClauseSyntax? elseClause) : base(tree)
	{
		IfToken = @if;
		Condition = condition;
		ThenStatement = thenStatement;
		ElseClause = elseClause;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitIfStatement(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitIfStatement(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return IfToken;
		yield return ThenStatement;
		if (ElseClause != null)
		{
			yield return ElseClause;
		}
	}
}