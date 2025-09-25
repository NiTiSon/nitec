using NUnit.Framework;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;

namespace NiteCompiler.Tests;

[TestFixture]
public class NiteLexerTests
{
    private static Token LexSingle(string text)
    {
        var diagnostics = new DiagnosticBag();
        var source = SourceText.From(text);
        var lexer = new NiteLexer(source, diagnostics);
        return lexer.Lex();
    }

    [Test]
    public void Lex_Identifier_ReturnsIdentifierToken()
    {
        var token = LexSingle("abc");
        Assert.That(token.Kind, Is.EqualTo(SyntaxKind.IdentifierToken));
    }

    [Test]
    public void Lex_Number_ReturnsNumberToken()
	{
		var token = LexSingle("12345");
		Assert.Multiple(() =>
		{
			Assert.That(token.Kind, Is.EqualTo(SyntaxKind.NumberToken));
			Assert.That(token, Is.TypeOf<TokenWithValue<int>>());
		});
	}

	[TestCase("+", SyntaxKind.PlusToken)]
    [TestCase("-", SyntaxKind.MinusToken)]
    [TestCase("->", SyntaxKind.RetusaToken)]
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
    [TestCase(".", SyntaxKind.DotToken)]
    [TestCase("..", SyntaxKind.DotDotToken)]
    [TestCase("..=", SyntaxKind.DotDotEqualsToken)]
    [TestCase("::", SyntaxKind.ColonColonToken)]
    [TestCase("?", SyntaxKind.QuestionToken)]
    [TestCase("??", SyntaxKind.QuestionQuestionToken)]
    [TestCase("??=", SyntaxKind.QuestionQuestionEqualsToken)]
    public void Lex_Symbols_ReturnsCorrectToken(string text, SyntaxKind expected)
    {
        var token = LexSingle(text);
        Assert.That(token.Kind, Is.EqualTo(expected));
    }

    [Test]
    public void Lex_InvalidCharacter_ReportsInvalidToken()
    {
        var token = LexSingle("@");
        Assert.That(token.Kind, Is.EqualTo(SyntaxKind.Invalid));
    }

    [Test]
    public void Lex_EndOfFile_ReturnsEofToken()
    {
        var diagnostics = new DiagnosticBag();
        var source = SourceText.From("");
        var lexer = new NiteLexer(source, diagnostics);
        var token = lexer.Lex();

        Assert.That(token.Kind, Is.EqualTo(SyntaxKind.EndOfFile));
    }
}
