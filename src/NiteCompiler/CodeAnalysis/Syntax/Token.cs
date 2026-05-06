using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

[DebuggerDisplay("{ToDebugString(), nq}")]
public abstract class Token : SyntaxNode
{
	public override TextSpan Span { get; }
	public SyntaxList<Trivia> LeadingTrivia { get; }
	public SyntaxList<Trivia> TrailingTrivia { get; }
	public abstract TokenKind TKind { get; }

	public sealed override NodeKind Kind
	{
		get
		{
			Debug.WriteLine("The TokenKind.Node is acquired, probably wrong behaviour.");
			return NodeKind.Token;
		}
	}
	public bool IsKeyword => TKind.IsKeyword;
	public bool IsPredefinedTypeKeyword => TKind.IsPredefinedTypeKeyword;

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

	public Token ToContextualKeywordToken()
	{
		TokenKind contextualToken = GetContextualKeyword();
		Debug.Assert(contextualToken != TokenKind.None);

		return new Default(Tree, contextualToken, Span, LeadingTrivia, TrailingTrivia);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitToken(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitToken(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return [];
	}

	public override string ToString()
	{
		return $"Token = {TKind} @{Span}";
	}

	private string ToDebugString()
	{
		TextLine? line = Tree.Text.Lines.GetLineByCharacterPosition(Span.Start);
		if (line == null)
		{
			return ToString();
		}

		return $"Token = {TKind} @{line.Value.HumanReadableLineNumber}";
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