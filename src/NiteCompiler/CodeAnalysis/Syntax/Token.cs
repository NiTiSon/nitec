using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class Token : SyntaxNode
{
	public override TextSpan Span { get; }
	public SyntaxList<Trivia> LeadingTrivia { get; }
	public SyntaxList<Trivia> TrailingTrivia { get; }
	public abstract TokenKind TKind { get; }
	public sealed override NodeKind Kind => NodeKind.Token;
	public bool IsKeyword => TKind.IsKeyword;
	public bool IsTypeKeyword => TKind.IsTypeKeyword;

	protected Token(SyntaxTree tree, TextSpan span, SyntaxList<Trivia> leadingTrivia, SyntaxList<Trivia> trailingTrivia) : base(tree)
	{
		LeadingTrivia = leadingTrivia;
		TrailingTrivia = trailingTrivia;
		Span = span;
	}

	public TokenKind GetContextualKeyword()
	{
		if (TKind == TokenKind.IdentifierOrKeyword) // IdentifierOrKeyword stores Keyword kind in high 16 bits
		{
			return TKind.HighBits;
		}

		return TokenKind.None;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return [];
	}

	public override string ToString()
	{
		return $"TokenKind = {TKind};";
	}

	public sealed class Default(
		SyntaxTree tree,
		TokenKind kind,
		TextSpan span,
		SyntaxList<Trivia> leadingTrivia,
		SyntaxList<Trivia> trailingTrivia) :
		Token(tree, span, leadingTrivia, trailingTrivia)
	{
		public override TokenKind TKind { get; } = kind;
	}
}