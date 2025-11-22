using NUnit.Framework;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Tests;

[TestFixture]
public class NiteLexerTests
{
	private static Token LexSingle(string text)
	{
		DiagnosticBag diagnostics = new();
		SourceText source = SourceText.From(text);
		NiteLexer lexer = new(source, diagnostics);
		return lexer.Lex();
	}

	[Test]
	public void Lex_Identifier_ReturnsIdentifierToken()
	{
		Token token = LexSingle("abc");
		Assert.That(token.Kind, Is.EqualTo(SyntaxKind.IdentifierToken));
	}

	[Test]
	public void Lex_Number_ReturnsNumberToken()
	{
		Token token = LexSingle("12345");
		Assert.Multiple(() =>
		{
			Assert.That(token.Kind, Is.EqualTo(SyntaxKind.NumberToken));
			Assert.That(token, Is.TypeOf<NumberToken>());
		});
	}

	[TestCase("->", SyntaxKind.RetusaToken)]
	[TestCase("+", SyntaxKind.PlusToken)]
	[TestCase("-", SyntaxKind.MinusToken)]
	[TestCase("*", SyntaxKind.AsteriskToken)]
	[TestCase("*=", SyntaxKind.AsteriskEqualsToken)]
	[TestCase("/", SyntaxKind.SlashToken)]
	[TestCase("/=", SyntaxKind.SlashEqualsToken)]
	[TestCase("=", SyntaxKind.EqualsToken)]
	[TestCase("==", SyntaxKind.EqualsEqualsToken)]
	[TestCase("!", SyntaxKind.ExclamationToken)]
	[TestCase("!=", SyntaxKind.ExclamationEqualsToken)]
	[TestCase(";", SyntaxKind.SemicolonToken)]
	[TestCase("(", SyntaxKind.OpenParenToken)]
	[TestCase(")", SyntaxKind.CloseParenToken)]
	[TestCase("{", SyntaxKind.OpenBraceToken)]
	[TestCase("}", SyntaxKind.CloseBraceToken)]
	[TestCase(",", SyntaxKind.CommaToken)]
	[TestCase("&", SyntaxKind.AmpersandToken)]
	[TestCase("&=", SyntaxKind.AmpersandEqualsToken)]
	[TestCase("&&", SyntaxKind.AmpersandAmpersandToken)]
	[TestCase("|", SyntaxKind.PipeToken)]
	[TestCase("|=", SyntaxKind.PipeEqualsToken)]
	[TestCase("||", SyntaxKind.PipePipeToken)]
	[TestCase("^", SyntaxKind.CaretToken)]
	[TestCase("^=", SyntaxKind.CaretEqualsToken)]
	[TestCase("<", SyntaxKind.LessThanToken)]
	[TestCase("<=", SyntaxKind.LessThanEqualsToken)]
	[TestCase(">", SyntaxKind.GreaterThanToken)]
	[TestCase(">=", SyntaxKind.GreaterThanEqualsToken)]
	[TestCase("<<", SyntaxKind.LeftShiftToken)]
	[TestCase("<<=", SyntaxKind.LeftShiftEqualsToken)]
	[TestCase(".", SyntaxKind.DotToken)]
	[TestCase("..", SyntaxKind.DotDotToken)]
	[TestCase("..=", SyntaxKind.DotDotEqualsToken)]
	[TestCase("::", SyntaxKind.ColonColonToken)]
	[TestCase("?", SyntaxKind.QuestionToken)]
	[TestCase("??", SyntaxKind.QuestionQuestionToken)]
	[TestCase("??=", SyntaxKind.QuestionQuestionEqualsToken)]
	public void Lex_Symbols_ReturnsCorrectToken(string text, SyntaxKind expected)
	{
		Token token = LexSingle(text);
		Assert.That(token.Kind, Is.EqualTo(expected));
	}

	[Test]
	public void Lex_InvalidCharacter_ReportsInvalidToken()
	{
		Token token = LexSingle("@");
		Assert.That(token.Kind, Is.EqualTo(SyntaxKind.None));
	}

	[Test]
	public void Lex_EndOfFile_ReturnsEofToken()
	{
		DiagnosticBag diagnostics = new();
		SourceText source = SourceText.From("");
		NiteLexer lexer = new(source, diagnostics);
		Token token = lexer.Lex();

		Assert.That(token.Kind, Is.EqualTo(SyntaxKind.EofToken));
	}
}