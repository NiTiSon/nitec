using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ExpressionStatementSyntax : StatementSyntax
{
	public ExpressionSyntax Expression { get; }
	public Token SemicolonToken { get; }
	public override TextSpan Span => Expression.Span;
	public override NodeKind Kind => NodeKind.ExpressionStatement;

	internal ExpressionStatementSyntax(SyntaxTree tree, ExpressionSyntax expression, Token semicolonToken) : base(tree)
	{
		Debug.Assert(semicolonToken.TKind == TokenKind.Semicolon);
		SemicolonToken = semicolonToken;
		Expression = expression;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitExpressionStatement(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitExpressionStatement(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Expression;
		yield return SemicolonToken;
	}
}