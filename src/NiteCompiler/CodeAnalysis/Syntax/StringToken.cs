using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

/// <summary>
/// Corresponds to the lexemes that own syntax can contain escape sequences.
/// </summary>
internal sealed class StringToken : Token
{
	public string Text { get; }
	public StringLiteralType LiteralType { get; }
	public override TokenKind TKind { get; }

	public StringToken(SyntaxTree tree, TokenKind kind, TextSpan span, string text, StringLiteralType literalType,
		SyntaxList<Trivia> leadingTrivia, SyntaxList<Trivia> trailingTrivia)
		: base(tree, span, leadingTrivia, trailingTrivia)
	{
		Debug.Assert(kind == TokenKind.StringLiteral ||
		             kind == TokenKind.CharacterLiteral ||
		             kind == TokenKind.EscapedIdentifier ||
		             kind == TokenKind.LifetimeIdentifier);

		Text = text;
		LiteralType = literalType;
		TKind = kind;
	}
}