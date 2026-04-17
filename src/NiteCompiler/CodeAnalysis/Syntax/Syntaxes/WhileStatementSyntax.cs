using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class WhileStatementSyntax : StatementSyntax
{
	public Token WhileKeyword { get; }
	public ExpressionSyntax Condition { get; }
	public StatementSyntax Body { get; }

	public override TextSpan Span => TextSpan.FromBounds(WhileKeyword.Span, Body.Span);
	public override NodeKind Kind => NodeKind.WhileStatement;

	public WhileStatementSyntax(SyntaxTree tree, Token whileKeyword, ExpressionSyntax condition, StatementSyntax body) : base(tree)
	{
		WhileKeyword = whileKeyword;
		Condition = condition;
		Body = body;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitWhileStatement(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitWhileStatement(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return WhileKeyword;
		yield return Body;
	}
}