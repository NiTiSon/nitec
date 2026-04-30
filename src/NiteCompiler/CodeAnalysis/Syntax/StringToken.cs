using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal sealed class StringToken : Token
{
	public string Text { get; }
	public override TokenKind TKind { get; }

	public StringToken(SyntaxTree tree, TokenKind kind, TextSpan span, string text,
		SyntaxList<Trivia> leadingTrivia, SyntaxList<Trivia> trailingTrivia)
		: base(tree, span, leadingTrivia, trailingTrivia)
	{
		Debug.Assert(kind == TokenKind.StringLiteral || kind == TokenKind.EscapedIdentifier);

		Text = text;
		TKind = kind;
	}
}