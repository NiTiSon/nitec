using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ReturnStatementSyntax : StatementSyntax
{
	public Token ReturnKeyword { get; }
	public ExpressionSyntax? Expression { get; }
	public Token SemicolonToken { get; }

	public override NodeKind Kind => NodeKind.ReturnStatement;
	public override TextSpan Span => TextSpan.FromBounds(ReturnKeyword.Span, SemicolonToken.Span);

	internal ReturnStatementSyntax(SyntaxTree tree, Token returnToken, ExpressionSyntax? expression, Token semicolonToken) : base(tree)
	{
		Debug.Assert(semicolonToken.TKind == TokenKind.Semicolon);
		SemicolonToken = semicolonToken;
		ReturnKeyword = returnToken;
		Expression = expression;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitReturnStatement(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitReturnStatement(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ReturnKeyword;
		if (Expression is not null)
		{
			yield return Expression;
		}
		yield return SemicolonToken;
	}
}