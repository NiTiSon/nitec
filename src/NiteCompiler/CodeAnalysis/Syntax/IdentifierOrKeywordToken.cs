using System.Reflection.Metadata;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class IdentifierOrKeywordToken : Token
{
	public string Identifier { get; }
	public override TokenKind TKind { get; }

	public IdentifierOrKeywordToken(SyntaxTree tree, TokenKind contextualKeyword, TextSpan span, string identifier,
		SyntaxList<Trivia> leadingTrivia, SyntaxList<Trivia> trailingTrivia) : base(tree, span, leadingTrivia, trailingTrivia)
	{
		Identifier = identifier;
		TKind = contextualKeyword;
	}

	public override string ToString()
	{
		return base.ToString() + $"Identifier: {Identifier} @{Span}";
	}
}