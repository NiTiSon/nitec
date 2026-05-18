using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class BreakStatementSyntax : StatementSyntax
{
	public Token BreakKeyword { get; }
	public Token SemicolonToken { get; }

	public override NodeKind Kind => NodeKind.BreakStatement;
	public override TextSpan Span => TextSpan.FromBounds(BreakKeyword.Span, SemicolonToken.Span);

	internal BreakStatementSyntax(SyntaxTree tree, Token breakToken, Token semicolonToken) : base(tree)
	{
		Debug.Assert(semicolonToken.TKind == TokenKind.Semicolon);
		SemicolonToken = semicolonToken;
		BreakKeyword = breakToken;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBreakStatement(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitBreakStatement(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return BreakKeyword;
		yield return SemicolonToken;
	}
}
