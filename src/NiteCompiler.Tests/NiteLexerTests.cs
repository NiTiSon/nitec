using NUnit.Framework;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Tests;

[TestFixture]
public class NiteLexerTests
{
	private static Token LexSingle(string text)
	{
		DiagnosticBag diagnostics = [];
		SyntaxTree tree = SyntaxTree.FromText(text, NiteCompilationOptions.Default, filename: "<tests>");
		NiteLexer lexer = new(tree, NiteCompilationOptions.Default, diagnostics);
		return lexer.Lex();
	}

	[Test]
	public void Lex_Identifier_ReturnsIdentifierToken()
	{
		Token token = LexSingle("abc");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_Number_ReturnsNumberToken()
	{
		Token token = LexSingle("12345");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
			Assert.That(token, Is.TypeOf<NumberToken>());
		});
	}


	[Test]
	public void Lex_InvalidCharacter_ReportsInvalidToken()
	{
		Token token = LexSingle("@");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.None));
	}

	[Test]
	public void Lex_EndOfFile_ReturnsEofToken()
	{
		DiagnosticBag diagnostics = [];
		SyntaxTree tree = SyntaxTree.FromText(string.Empty, NiteCompilationOptions.Default, "<test>");
		NiteLexer lexer = new(tree, NiteCompilationOptions.Default, diagnostics);
		Token token = lexer.Lex();

		Assert.That(token.TKind, Is.EqualTo(TokenKind.EndOfFile));
	}
}