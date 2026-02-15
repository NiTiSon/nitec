using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class EmptyStatementSyntax : StatementSyntax
{
	public Token SemicolonToken { get; }

	public override TextSpan Span => SemicolonToken.Span;
	public override NodeKind Kind => NodeKind.EmptyStatement;

	public EmptyStatementSyntax(SyntaxTree tree, Token semicolonToken) : base(tree)
	{
		Debug.Assert(semicolonToken.TKind == TokenKind.Semicolon);
		SemicolonToken = semicolonToken;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEmptyStatement(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitEmptyStatement(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return SemicolonToken;
	}
}