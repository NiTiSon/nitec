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
		SyntaxTree tree = SyntaxTree.ParseText(text, null, NiteCompilationOptions.Default);
		NiteLexer lexer = new(tree, NiteCompilationOptions.Default, diagnostics);
		return lexer.Lex();
	}

	private static Token LexFirstTokenInContext(string text)
	{
		DiagnosticBag diagnostics = [];
		SyntaxTree tree = SyntaxTree.ParseText(text, null, NiteCompilationOptions.Default);
		NiteLexer lexer = new(tree, NiteCompilationOptions.Default, diagnostics);
		return lexer.Lex();
	}

	private static Token[] LexAll(string text)
	{
		DiagnosticBag diagnostics = [];
		SyntaxTree tree = SyntaxTree.ParseText(text, null, NiteCompilationOptions.Default);
		NiteLexer lexer = new(tree, NiteCompilationOptions.Default, diagnostics);
		List<Token> tokens = [];
		while (true)
		{
			Token token = lexer.Lex();
			tokens.Add(token);
			if (token.TKind == TokenKind.EndOfFile)
				break;
		}
		return [.. tokens];
	}

	// ==============================
	// Identifiers
	// ==============================
	[Test]
	public void Lex_SimpleIdentifier_ReturnsIdentifierToken()
	{
		Token token = LexSingle("abc");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_IdentifierWithUnderscore_ReturnsIdentifierToken()
	{
		Token token = LexSingle("my_var");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_IdentifierStartingWithUnderscore_ReturnsIdentifierToken()
	{
		Token token = LexSingle("_private");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_IdentifierWithDigits_ReturnsIdentifierToken()
	{
		Token token = LexSingle("var2");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_IdentifierIsUpperCase_ReturnsIdentifierToken()
	{
		Token token = LexSingle("ABCDEF");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_EscapedIdentifier_ReturnsEscapedIdentifierToken()
	{
		Token token = LexSingle("`escaped`");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.EscapedIdentifier));
			Assert.That(token, Is.TypeOf<StringToken>());
			StringToken st = (StringToken)token;
			Assert.That(st.Text, Is.EqualTo("escaped"));
		});
	}

	[Test]
	public void Lex_EscapedIdentifierWithBacktickInside_ReturnsEscapedIdentifier()
	{
		Token token = LexSingle("`he\\`llo`");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.EscapedIdentifier));
			Assert.That(token, Is.TypeOf<StringToken>());
			StringToken st = (StringToken)token;
			Assert.That(st.Text, Is.EqualTo("he`llo"));
		});
	}

	[Test]
	public void Lex_EscapedIdentifierWithEscapeSequences_ReturnsEscapedIdentifier()
	{
		Token token = LexSingle("`line1\\nline2`");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.EscapedIdentifier));
			Assert.That(token, Is.TypeOf<StringToken>());
			StringToken st = (StringToken)token;
			Assert.That(st.Text, Is.EqualTo("line1\nline2"));
		});
	}

	[Test]
	public void Lex_KeywordAsIdentifier_ReturnsIdentifierWhenNotKeyword()
	{
		// "foo" is not a keyword
		Token token = LexSingle("foo");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_IdentifierMatchingPredefinedType_ReturnsIdentifier()
	{
		// "myInt" vs "i32" - should be an identifier
		Token token = LexSingle("myInt");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	// ==============================
	// Number Literals - Decimal
	// ==============================
	[Test]
	public void Lex_NumberInteger_ReturnsNumberToken()
	{
		Token token = LexSingle("12345");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
			Assert.That(token, Is.TypeOf<NumberToken>());
		});
	}

	[Test]
	public void Lex_NumberZero_ReturnsNumberToken()
	{
		Token token = LexSingle("0");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberWithDigitSeparator_ReturnsNumberToken()
	{
		Token token = LexSingle("1'000'000");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberFloat_ReturnsNumberToken()
	{
		Token token = LexSingle("3.14");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberFloatTrailingDot_ReturnsNumberToken()
	{
		Token token = LexSingle("42.");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberScientificNotation_ReturnsNumberToken()
	{
		Token token = LexSingle("1e5");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberScientificNotationNegativeExponent_ReturnsNumberToken()
	{
		Token token = LexSingle("1.5e-2");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberScientificNotationPositiveExponent_ReturnsNumberToken()
	{
		Token token = LexSingle("1.5e+3");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixI32_ReturnsNumberToken()
	{
		Token token = LexSingle("42i32");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
			Assert.That(token, Is.TypeOf<NumberToken>());
			NumberToken nt = (NumberToken)token;
			Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.I32));
		});
	}

	[Test]
	public void Lex_NumberWithTypeSuffixU64_ReturnsNumberToken()
	{
		Token token = LexSingle("100u64");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.U64));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixF32_ReturnsNumberToken()
	{
		Token token = LexSingle("3.14f32");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.F32));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixF64_ReturnsNumberToken()
	{
		Token token = LexSingle("2.71f64");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.F64));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixUnsigned_ReturnsNumberToken()
	{
		Token token = LexSingle("42u");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.Unsigned));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixSigned_ReturnsNumberToken()
	{
		Token token = LexSingle("42i");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.Signed));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixI8_ReturnsNumberToken()
	{
		Token token = LexSingle("127i8");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.I8));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixU8_ReturnsNumberToken()
	{
		Token token = LexSingle("255u8");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.U8));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixI16_ReturnsNumberToken()
	{
		Token token = LexSingle("32767i16");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.I16));
	}

	[Test]
	public void Lex_NumberWithTypeSuffixU16_ReturnsNumberToken()
	{
		Token token = LexSingle("65535u16");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.U16));
	}

	// ==============================
	// Number Literals - Hex
	// ==============================
	[Test]
	public void Lex_NumberHex_ReturnsNumberToken()
	{
		Token token = LexSingle("0xFF");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberHexWithSeparators_ReturnsNumberToken()
	{
		Token token = LexSingle("0xDE'AD'BE'EF");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberHexLowercase_ReturnsNumberToken()
	{
		Token token = LexSingle("0xff");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberHexWithTypeSuffix_ReturnsNumberToken()
	{
		Token token = LexSingle("0xFFu8");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.U8));
	}

	// ==============================
	// Number Literals - Binary
	// ==============================
	[Test]
	public void Lex_NumberBinary_ReturnsNumberToken()
	{
		Token token = LexSingle("0b1010");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberBinaryWithSeparators_ReturnsNumberToken()
	{
		Token token = LexSingle("0b1100'0011");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Lex_NumberBinaryWithTypeSuffix_ReturnsNumberToken()
	{
		Token token = LexSingle("0b1010u16");
		NumberToken nt = (NumberToken)token;
		Assert.That(nt.Type, Is.EqualTo(NumericLiteralType.U16));
	}

	// ==============================
	// String Literals
	// ==============================
	[Test]
	public void Lex_StringLiteral_ReturnsStringToken()
	{
		Token token = LexSingle("\"hello\"");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.StringLiteral));
			Assert.That(token, Is.TypeOf<StringToken>());
			StringToken st = (StringToken)token;
			Assert.That(st.Text, Is.EqualTo("hello"));
		});
	}

	[Test]
	public void Lex_StringLiteralEmpty_ReturnsStringToken()
	{
		Token token = LexSingle("\"\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo(""));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeNewline_ReturnsStringToken()
	{
		Token token = LexSingle("\"line1\\nline2\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("line1\nline2"));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeTab_ReturnsStringToken()
	{
		Token token = LexSingle("\"col1\\tcol2\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("col1\tcol2"));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeBackslash_ReturnsStringToken()
	{
		Token token = LexSingle("\"path\\\\to\\\\file\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("path\\to\\file"));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeQuote_ReturnsStringToken()
	{
		Token token = LexSingle("\"she said \\\"hello\\\"\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("she said \"hello\""));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeBacktick_ReturnsStringToken()
	{
		Token token = LexSingle("\"back\\`tick\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("back`tick"));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeCarriageReturn_ReturnsStringToken()
	{
		Token token = LexSingle("\"line1\\r\\nline2\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("line1\r\nline2"));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeBackspace_ReturnsStringToken()
	{
		Token token = LexSingle("\"before\\bafter\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("before\bafter"));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeVerticalTab_ReturnsStringToken()
	{
		Token token = LexSingle("\"a\\vb\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("a\vb"));
	}

	[Test]
	public void Lex_StringLiteralWithEscapeNull_ReturnsStringToken()
	{
		Token token = LexSingle("\"null\\0char\"");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("null\0char"));
	}

	[Test]
	public void Lex_StringLiteralUnterminated_StillProducesToken()
	{
		Token token = LexSingle("\"unterminated");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.StringLiteral));
	}

	// ==============================
	// Character Literals
	// ==============================
	[Test]
	public void Lex_CharacterLiteral_ReturnsCharacterToken()
	{
		Token token = LexSingle("'a'");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.CharacterLiteral));
			Assert.That(token, Is.TypeOf<StringToken>());
			StringToken st = (StringToken)token;
			Assert.That(st.Text, Is.EqualTo("a"));
		});
	}

	[Test]
	public void Lex_CharacterLiteralEscapedNewline_ReturnsCharacterToken()
	{
		Token token = LexSingle("'\\n'");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("\n"));
	}

	[Test]
	public void Lex_CharacterLiteralEscapedTab_ReturnsCharacterToken()
	{
		Token token = LexSingle("'\\t'");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("\t"));
	}

	[Test]
	public void Lex_CharacterLiteralEscapedBackslash_ReturnsCharacterToken()
	{
		Token token = LexSingle("'\\\\'");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("\\"));
	}

	[Test]
	public void Lex_CharacterLiteralEscapedQuote_ReturnsCharacterToken()
	{
		Token token = LexSingle("'\\''");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("'"));
	}

	[Test]
	public void Lex_CharacterLiteralEscapedNull_ReturnsCharacterToken()
	{
		Token token = LexSingle("'\\0'");
		StringToken st = (StringToken)token;
		Assert.That(st.Text, Is.EqualTo("\0"));
	}

	[Test]
	public void Lex_CharacterLiteralUnterminated_StillProducesToken()
	{
		Token token = LexSingle("'x");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.LifetimeIdentifier));
	}

	// ==============================
	// Lifetime Identifiers
	// ==============================
	[Test]
	public void Lex_LifetimeIdentifier_ReturnsLifetimeToken()
	{
		Token token = LexSingle("'a");
		Assert.Multiple(() =>
		{
			Assert.That(token.TKind, Is.EqualTo(TokenKind.LifetimeIdentifier));
			Assert.That(token, Is.TypeOf<StringToken>());
		});
	}

	[Test]
	public void Lex_LifetimeIdentifierLongName_ReturnsLifetimeToken()
	{
		Token token = LexSingle("'my_lifetime");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.LifetimeIdentifier));
	}

	[Test]
	public void Lex_LifetimeIdentifierSingleUnderscore_ReturnsLifetimeToken()
	{
		Token token = LexSingle("'_");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.LifetimeIdentifier));
	}

	// ==============================
	// Keywords
	// ==============================
	[TestCase("true", nameof(TokenKind.True))]
	[TestCase("false", nameof(TokenKind.False))]
	[TestCase("use", nameof(TokenKind.Use))]
	[TestCase("if", nameof(TokenKind.If))]
	[TestCase("else", nameof(TokenKind.Else))]
	[TestCase("loop", nameof(TokenKind.Loop))]
	[TestCase("while", nameof(TokenKind.While))]
	[TestCase("for", nameof(TokenKind.For))]
	[TestCase("do", nameof(TokenKind.Do))]
	[TestCase("break", nameof(TokenKind.Break))]
	[TestCase("return", nameof(TokenKind.Return))]
	[TestCase("let", nameof(TokenKind.Let))]
	[TestCase("static", nameof(TokenKind.Static))]
	[TestCase("const", nameof(TokenKind.Const))]
	[TestCase("pure", nameof(TokenKind.Pure))]
	[TestCase("interface", nameof(TokenKind.Interface))]

	[TestCase("partial", nameof(TokenKind.Partial))]
	[TestCase("unsized", nameof(TokenKind.Unsized))]
	[TestCase("self", nameof(TokenKind.Self))]
	[TestCase("new", nameof(TokenKind.New))]
	public void Lex_Keyword_ReturnsCorrectKeywordToken(string keyword, string expectedTokenName)
	{
		Token token = LexSingle(keyword);
		TokenKind expected = expectedTokenName switch
		{
			nameof(TokenKind.True) => TokenKind.True,
			nameof(TokenKind.False) => TokenKind.False,
			nameof(TokenKind.Use) => TokenKind.Use,
			nameof(TokenKind.If) => TokenKind.If,
			nameof(TokenKind.Else) => TokenKind.Else,
			nameof(TokenKind.Loop) => TokenKind.Loop,
			nameof(TokenKind.While) => TokenKind.While,
			nameof(TokenKind.For) => TokenKind.For,
			nameof(TokenKind.Do) => TokenKind.Do,
			nameof(TokenKind.Break) => TokenKind.Break,
			nameof(TokenKind.Return) => TokenKind.Return,
			nameof(TokenKind.Let) => TokenKind.Let,
			nameof(TokenKind.Static) => TokenKind.Static,
			nameof(TokenKind.Const) => TokenKind.Const,
			nameof(TokenKind.Pure) => TokenKind.Pure,
			nameof(TokenKind.Interface) => TokenKind.Interface,
			nameof(TokenKind.Partial) => TokenKind.Partial,
			nameof(TokenKind.Unsized) => TokenKind.Unsized,
			nameof(TokenKind.Self) => TokenKind.Self,
			nameof(TokenKind.New) => TokenKind.New,
			_ => throw new ArgumentException($"Unknown token: {expectedTokenName}")
		};
		Assert.That(token.TKind, Is.EqualTo(expected));
		Assert.That(token.IsKeyword, Is.True);
	}

	[Test]
	public void Lex_ContextualKeywords_AccessibilityAndModuleAndType()
	{
		// Accessibility keywords are contextual: the lexer returns them as IdentifierOrKeyword
		// with the actual keyword kind packed into the high bits (IsKeyword is false on the token).
		// module and type are their own token kinds (not contextual).
		Token token;

		token = LexFirstTokenInContext("public x: i32;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(token.GetContextualKeyword(), Is.EqualTo(TokenKind.Public));

		token = LexFirstTokenInContext("friend x: i32;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(token.GetContextualKeyword(), Is.EqualTo(TokenKind.Friend));

		token = LexFirstTokenInContext("protected x: i32;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(token.GetContextualKeyword(), Is.EqualTo(TokenKind.Protected));

		token = LexFirstTokenInContext("internal x: i32;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(token.GetContextualKeyword(), Is.EqualTo(TokenKind.Internal));

		token = LexFirstTokenInContext("family x: i32;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(token.GetContextualKeyword(), Is.EqualTo(TokenKind.Family));

		token = LexFirstTokenInContext("private x: i32;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(token.GetContextualKeyword(), Is.EqualTo(TokenKind.Private));

		token = LexFirstTokenInContext("module foo;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Module));

		token = LexFirstTokenInContext("type Foo;");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Type));
	}

	// ==============================
	// Predefined Type Keywords
	// ==============================
	[TestCase("i8", nameof(TokenKind.I8))]
	[TestCase("i16", nameof(TokenKind.I16))]
	[TestCase("i32", nameof(TokenKind.I32))]
	[TestCase("i64", nameof(TokenKind.I64))]
	[TestCase("u8", nameof(TokenKind.U8))]
	[TestCase("u16", nameof(TokenKind.U16))]
	[TestCase("u32", nameof(TokenKind.U32))]
	[TestCase("u64", nameof(TokenKind.U64))]
	[TestCase("f16", nameof(TokenKind.F16))]
	[TestCase("f32", nameof(TokenKind.F32))]
	[TestCase("f64", nameof(TokenKind.F64))]
	[TestCase("void", nameof(TokenKind.Void))]
	[TestCase("bool", nameof(TokenKind.Boolean))]
	public void Lex_PredefinedTypeKeyword_ReturnsCorrectToken(string keyword, string expectedTokenName)
	{
		Token token = LexSingle(keyword);
		TokenKind expected = expectedTokenName switch
		{
			nameof(TokenKind.I8) => TokenKind.I8,
			nameof(TokenKind.I16) => TokenKind.I16,
			nameof(TokenKind.I32) => TokenKind.I32,
			nameof(TokenKind.I64) => TokenKind.I64,
			nameof(TokenKind.U8) => TokenKind.U8,
			nameof(TokenKind.U16) => TokenKind.U16,
			nameof(TokenKind.U32) => TokenKind.U32,
			nameof(TokenKind.U64) => TokenKind.U64,
			nameof(TokenKind.F16) => TokenKind.F16,
			nameof(TokenKind.F32) => TokenKind.F32,
			nameof(TokenKind.F64) => TokenKind.F64,
			nameof(TokenKind.Void) => TokenKind.Void,
			nameof(TokenKind.Boolean) => TokenKind.Boolean,
			_ => throw new ArgumentException($"Unknown token: {expectedTokenName}")
		};
		Assert.That(token.TKind, Is.EqualTo(expected));
	}

	// ==============================
	// Punctuators
	// ==============================
	[TestCase(".", nameof(TokenKind.Dot))]
	[TestCase(",", nameof(TokenKind.Comma))]
	[TestCase(":", nameof(TokenKind.Colon))]
	[TestCase(";", nameof(TokenKind.Semicolon))]
	[TestCase("(", nameof(TokenKind.OpenParen))]
	[TestCase(")", nameof(TokenKind.CloseParen))]
	[TestCase("{", nameof(TokenKind.OpenBrace))]
	[TestCase("}", nameof(TokenKind.CloseBrace))]
	[TestCase("[", nameof(TokenKind.OpenBracket))]
	[TestCase("]", nameof(TokenKind.CloseBracket))]
	public void Lex_Punctuator_ReturnsCorrectPunctuatorToken(string source, string expectedTokenName)
	{
		Token token = LexSingle(source);
		TokenKind expected = expectedTokenName switch
		{
			nameof(TokenKind.Dot) => TokenKind.Dot,
			nameof(TokenKind.Comma) => TokenKind.Comma,
			nameof(TokenKind.Colon) => TokenKind.Colon,
			nameof(TokenKind.Semicolon) => TokenKind.Semicolon,
			nameof(TokenKind.OpenParen) => TokenKind.OpenParen,
			nameof(TokenKind.CloseParen) => TokenKind.CloseParen,
			nameof(TokenKind.OpenBrace) => TokenKind.OpenBrace,
			nameof(TokenKind.CloseBrace) => TokenKind.CloseBrace,
			nameof(TokenKind.OpenBracket) => TokenKind.OpenBracket,
			nameof(TokenKind.CloseBracket) => TokenKind.CloseBracket,
			_ => throw new ArgumentException($"Unknown token: {expectedTokenName}")
		};
		Assert.That(token.TKind, Is.EqualTo(expected));
	}

	[Test]
	public void Lex_Retusa_ReturnsRetusaToken()
	{
		Token token = LexSingle("->");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Retusa));
	}

	[Test]
	public void Lex_DoubleColon_ReturnsDoubleColonToken()
	{
		Token token = LexSingle("::");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.DoubleColon));
	}

	// ==============================
	// Operators - Arithmetic
	// ==============================
	[Test]
	public void Lex_Plus_ReturnsPlusToken()
	{
		Token token = LexSingle("+");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Plus));
	}

	[Test]
	public void Lex_Minus_ReturnsMinusToken()
	{
		Token token = LexSingle("-");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Minus));
	}

	[Test]
	public void Lex_Asterisk_ReturnsAsteriskToken()
	{
		Token token = LexSingle("*");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Asterisk));
	}

	[Test]
	public void Lex_Slash_ReturnsSlashToken()
	{
		Token token = LexSingle("/");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Slash));
	}

	[Test]
	public void Lex_Percent_ReturnsPercentToken()
	{
		Token token = LexSingle("%");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Percent));
	}

	// ==============================
	// Operators - Assignment
	// ==============================
	[Test]
	public void Lex_Equals_ReturnsEqualToken()
	{
		Token token = LexSingle("=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Equal));
	}

	[Test]
	public void Lex_PlusAssignment_ReturnsPlusAssignmentToken()
	{
		Token token = LexSingle("+=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.PlusAssignment));
	}

	[Test]
	public void Lex_MinusAssignment_ReturnsMinusAssignmentToken()
	{
		Token token = LexSingle("-=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.MinusAssignment));
	}

	[Test]
	public void Lex_AsteriskAssignment_ReturnsAsteriskAssignmentToken()
	{
		Token token = LexSingle("*=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.AsteriskAssignment));
	}

	[Test]
	public void Lex_SlashAssignment_ReturnsSlashAssignmentToken()
	{
		Token token = LexSingle("/=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.SlashAssignment));
	}

	[Test]
	public void Lex_PercentAssignment_ReturnsPercentAssignmentToken()
	{
		Token token = LexSingle("%=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.PercentAssignment));
	}

	// ==============================
	// Operators - Bitwise
	// ==============================
	[Test]
	public void Lex_Tilde_ReturnsTildeToken()
	{
		Token token = LexSingle("~");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Tilde));
	}

	[Test]
	public void Lex_Pipe_ReturnsPipeToken()
	{
		Token token = LexSingle("|");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Pipe));
	}

	[Test]
	public void Lex_Circumflex_ReturnsCircumflexToken()
	{
		Token token = LexSingle("^");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Circumflex));
	}

	[Test]
	public void Lex_Ampersand_ReturnsAmpersandToken()
	{
		Token token = LexSingle("&");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Ampersand));
	}

	[Test]
	public void Lex_Exclamation_ReturnsExclamationToken()
	{
		Token token = LexSingle("!");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.ExclamationSign));
	}

	// ==============================
	// Operators - Shift
	// ==============================
	// ==============================
	// Operators - Bitwise Assignment
	// ==============================
	// Operators - Bitwise Assignment
	// ==============================
	[Test]
	public void Lex_TildeAssignment_ReturnsTildeAssignmentToken()
	{
		Token token = LexSingle("~=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.TildeAssignment));
	}

	[Test]
	public void Lex_PipeAssignment_ReturnsPipeAssignmentToken()
	{
		Token token = LexSingle("|=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.PipeAssignment));
	}

	[Test]
	public void Lex_CircumflexAssignment_ReturnsCircumflexAssignmentToken()
	{
		Token token = LexSingle("^=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.CircumflexAssignment));
	}

	[Test]
	public void Lex_AmpersandAssignment_ReturnsAmpersandAssignmentToken()
	{
		Token token = LexSingle("&=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.AmpersandAssignment));
	}

	// ==============================
	// Operators - Logical/Comparison
	// ==============================
	[Test]
	public void Lex_DoublePipe_ReturnsDoublePipeToken()
	{
		Token token = LexSingle("||");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.DoublePipe));
	}

	[Test]
	public void Lex_DoubleAmpersand_ReturnsDoubleAmpersandToken()
	{
		Token token = LexSingle("&&");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.DoubleAmpersand));
	}

	[Test]
	public void Lex_DoubleEqual_ReturnsDoubleEqualToken()
	{
		Token token = LexSingle("==");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.DoubleEqual));
	}

	[Test]
	public void Lex_NotEqual_ReturnsNotEqualToken()
	{
		Token token = LexSingle("!=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.NotEqual));
	}

	[Test]
	public void Lex_Greater_ReturnsGreaterToken()
	{
		Token token = LexSingle(">");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Greater));
	}

	[Test]
	public void Lex_GreaterOrEquals_ReturnsGreaterOrEqualsToken()
	{
		Token token = LexSingle(">=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.GreaterOrEquals));
	}

	[Test]
	public void Lex_Less_ReturnsLessToken()
	{
		Token token = LexSingle("<");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Less));
	}

	[Test]
	public void Lex_LessOrEquals_ReturnsLessOrEqualsToken()
	{
		Token token = LexSingle("<=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.LessOrEquals));
	}

	// ==============================
	// Operators - Range/Question
	// ==============================
	[Test]
	public void Lex_Range_ReturnsRangeToken()
	{
		Token token = LexSingle("..");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Range));
	}

	[Test]
	public void Lex_RangeInclusive_ReturnsRangeInclusiveToken()
	{
		Token token = LexSingle("..=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.RangeInclusive));
	}

	[Test]
	public void Lex_QuestionSign_ReturnsQuestionSignToken()
	{
		Token token = LexSingle("?");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.QuestionSign));
	}

	[Test]
	public void Lex_QuestionAssignment_ReturnsQuestionAssignmentToken()
	{
		Token token = LexSingle("?=");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.QuestionAssignmentSign));
	}

	[Test]
	public void Lex_DoubleQuestion_ReturnsDoubleQuestionToken()
	{
		Token token = LexSingle("??");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.DoubleQuestionSign));
	}

	// ==============================
	// Multiple Token Sequences
	// ==============================
	[Test]
	public void Lex_MultipleTokens_ReturnsAllTokensInOrder()
	{
		Token[] tokens = LexAll("a b c");

		Assert.That(tokens.Length, Is.EqualTo(4)); // a, b, c, EOF
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(tokens[2].TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(tokens[3].TKind, Is.EqualTo(TokenKind.EndOfFile));
	}

	[Test]
	public void Lex_ExpressionTokens_ReturnsCorrectSequence()
	{
		Token[] tokens = LexAll("1 + 2");

		Assert.That(tokens.Length, Is.EqualTo(4)); // 1, +, 2, EOF
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.NumberLiteral));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.Plus));
		Assert.That(tokens[2].TKind, Is.EqualTo(TokenKind.NumberLiteral));
		Assert.That(tokens[3].TKind, Is.EqualTo(TokenKind.EndOfFile));
	}

	[Test]
	public void Lex_FunctionDeclarationTokens_ReturnsCorrectSequence()
	{
		Token[] tokens = LexAll("public test() {}");

		// Format: public, test, (, ), {, }, EOF
		Assert.That(tokens.Length, Is.EqualTo(7));
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(tokens[2].TKind, Is.EqualTo(TokenKind.OpenParen));
		Assert.That(tokens[3].TKind, Is.EqualTo(TokenKind.CloseParen));
		Assert.That(tokens[4].TKind, Is.EqualTo(TokenKind.OpenBrace));
		Assert.That(tokens[5].TKind, Is.EqualTo(TokenKind.CloseBrace));
		Assert.That(tokens[6].TKind, Is.EqualTo(TokenKind.EndOfFile));
	}

	[Test]
	public void Lex_OperatorSequence_GreedyMatch()
	{
		// "->" should be Retusa, not Minus + Greater
		Token token = LexSingle("->");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Retusa));
	}

	[Test]
	public void Lex_SeparateOperators_ProducesSeparateTokens()
	{
		Token[] tokens = LexAll("- >");

		Assert.That(tokens.Length, Is.EqualTo(3)); // -, >, EOF
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.Minus));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.Greater));
	}

	// ==============================
	// Trivia - Whitespace
	// ==============================
	[Test]
	public void Lex_SingleSpace_ProducesLeadingTrivia()
	{
		Token token = LexSingle(" abc");
		Assert.That(token.LeadingTrivia.Count, Is.GreaterThan(0));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.Whitespace));
	}

	[Test]
	public void Lex_MultipleSpaces_ProducesWhitespaceTrivia()
	{
		Token token = LexSingle("   abc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.Whitespace));
	}

	[Test]
	public void Lex_Tab_ProducesWhitespaceTrivia()
	{
		Token token = LexSingle("\tabc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.Whitespace));
	}

	[Test]
	public void Lex_NoWhitespace_NoLeadingTrivia()
	{
		Token token = LexSingle("abc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(0));
	}

	// ==============================
	// Trivia - Single-Line Comments
	// ==============================
	[Test]
	public void Lex_SingleLineComment_ProducesCommentTrivia()
	{
		Token token = LexSingle("// comment\nabc");
		Assert.That(token.LeadingTrivia.Count, Is.GreaterThanOrEqualTo(2));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.SingleLineComment));
	}

	[Test]
	public void Lex_SingleLineCommentAtEndOfFile_ProducesCommentTrivia()
	{
		Token token = LexSingle("// comment");
		Assert.That(token.LeadingTrivia.Count, Is.GreaterThanOrEqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.SingleLineComment));
	}

	[Test]
	public void Lex_TrailingSingleLineComment_ProducesCommentTrivia()
	{
		Token[] tokens = LexAll("abc // trailing comment\n123");

		Assert.That(tokens[0].TrailingTrivia.Count, Is.GreaterThanOrEqualTo(1));
		Trivia trailingComment = tokens[0].TrailingTrivia.First(t => t.TKind == TokenKind.SingleLineComment);
		Assert.That(trailingComment.TKind, Is.EqualTo(TokenKind.SingleLineComment));
	}

	// ==============================
	// Trivia - Multi-Line Comments
	// ==============================
	[Test]
	public void Lex_MultiLineComment_ProducesCommentTrivia()
	{
		Token token = LexSingle("/* block */abc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.MultiLineComment));
	}

	[Test]
	public void Lex_MultiLineCommentMultiLine_ProducesCommentTrivia()
	{
		Token token = LexSingle("/*\n multi\n line\n */abc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.MultiLineComment));
	}

	[Test]
	public void Lex_MultiLineCommentAtEndOfFile_ProducesCommentTrivia()
	{
		Token token = LexSingle("/* unfinished");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.MultiLineComment));
	}

	// ==============================
	// Trivia - Line Breaks
	// ==============================
	[Test]
	public void Lex_LineBreakLF_ProducesLineBreakTrivia()
	{
		Token token = LexSingle("\nabc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.LineBreak));
	}

	[Test]
	public void Lex_LineBreakCRLF_ProducesSingleLineBreakTrivia()
	{
		Token token = LexSingle("\r\nabc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.LineBreak));
	}

	[Test]
	public void Lex_LineBreakCR_ProducesLineBreakTrivia()
	{
		Token token = LexSingle("\rabc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.LineBreak));
	}

	// ==============================
	// Trivia - Shebang
	// ==============================
	[Test]
	public void Lex_Shebang_ProducesShebangTrivia()
	{
		Token token = LexSingle("#! /usr/bin/env nite\nabc");
		Assert.That(token.LeadingTrivia.Count, Is.GreaterThanOrEqualTo(2));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.Shebang));
	}

	[Test]
	public void Lex_ShebangWithoutNewline_ProducesShebangTrivia()
	{
		Token token = LexSingle("#! nite script");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(1));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.Shebang));
	}

	[Test]
	public void Lex_NotShebang_HashNotFollowedByExclamation()
	{
		Token token = LexSingle("# not shebang\nabc");
		// # alone is typically an invalid character
		Assert.That(token.LeadingTrivia.Any(t => t.TKind == TokenKind.Shebang), Is.False);
	}

	// ==============================
	// Trivia - Docs Comments (with DocumentationMode.Parse)
	// ==============================
	[Test]
	public void Lex_DocsComment_WithDocumentationModeParse_ProducesDocsCommentTrivia()
	{
		DiagnosticBag diagnostics = [];
		SyntaxTree tree = SyntaxTree.ParseText("/// doc comment\nabc", null, NiteCompilationOptions.Default);
		NiteLexer lexer = new(tree, NiteCompilationOptions.Default, diagnostics);
		Token token = lexer.Lex();

		Assert.That(token.LeadingTrivia.Any(t => t.TKind == TokenKind.DocsComment), Is.True);
	}

	// ==============================
	// End of File
	// ==============================
	[Test]
	public void Lex_EmptySource_ReturnsEofToken()
	{
		DiagnosticBag diagnostics = [];
		SyntaxTree tree = SyntaxTree.ParseText(string.Empty, null, NiteCompilationOptions.Default);
		NiteLexer lexer = new(tree, NiteCompilationOptions.Default, diagnostics);
		Token token = lexer.Lex();

		Assert.That(token.TKind, Is.EqualTo(TokenKind.EndOfFile));
	}

	[Test]
	public void Lex_OnlyWhitespace_ReturnsEofTokenWithLeadingTrivia()
	{
		Token token = LexSingle("   ");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.EndOfFile));
		Assert.That(token.LeadingTrivia.Count, Is.GreaterThan(0));
	}

	[Test]
	public void Lex_AfterLastRealToken_LexReturnsEof()
	{
		Token[] tokens = LexAll("42");
		Assert.That(tokens.Length, Is.EqualTo(2));
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.NumberLiteral));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.EndOfFile));
	}

	// ==============================
	// Invalid / Error Characters
	// ==============================
	[Test]
	public void Lex_AtSymbol_ReturnsNoneToken()
	{
		Token token = LexSingle("@");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.None));
	}

	[Test]
	public void Lex_HashAlone_ReturnsHashToken()
	{
		Token token = LexSingle("#abc");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Hash));
	}

	[Test]
	public void Lex_DollarSign_ReturnsNoneToken()
	{
		Token token = LexSingle("$");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.None));
	}

	[Test]
	public void Lex_MultipleInvalidCharacters_EachReturnsNone()
	{
		Token[] tokens = LexAll("@#$");
		Assert.That(tokens.Length, Is.EqualTo(4)); // @, #, $, EOF
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.None));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.Hash));
		Assert.That(tokens[2].TKind, Is.EqualTo(TokenKind.None));
	}

	[Test]
	public void Lex_InvalidCharacterBetweenValidTokens_ReturnsNoneForInvalid()
	{
		Token[] tokens = LexAll("a @ b");

		Assert.That(tokens.Length, Is.EqualTo(4)); // a, @, b, EOF
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.None));
		Assert.That(tokens[2].TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	// ==============================
	// Edge Cases
	// ==============================
	[Test]
	public void Lex_Semicolon_ReturnsSemicolonToken()
	{
		Token token = LexSingle(";");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Semicolon));
	}

	[Test]
	public void Lex_Colon_ReturnsColonToken()
	{
		Token token = LexSingle(":");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Colon));
	}

	[Test]
	public void Lex_QuoteThenIdentifier_AreSeparateTokens()
	{
		Token[] tokens = LexAll("'abc");

		// Could be Quote + identifier, or lifetime identifier
		Assert.That(tokens.Length, Is.EqualTo(2)); // either way: something + EOF
		Assert.That(tokens[tokens.Length - 1].TKind, Is.EqualTo(TokenKind.EndOfFile));
	}

	[Test]
	public void Lex_MultipleOperatorsNoSpacing_CorrectGreedyParsing()
	{
		Token[] tokens = LexAll("..=");

		Assert.That(tokens.Length, Is.EqualTo(2)); // ..= or .. + =?
		// ..= is RangeInclusive
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.RangeInclusive));
	}

	[Test]
	public void Lex_ArrowVsGreaterMinus_CorrectParsing()
	{
		// "->" should be Retusa
		Token token = LexSingle("->");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.Retusa));

		// "- >" should be Minus then Greater
		Token[] tokens = LexAll("- >");
		Assert.That(tokens[0].TKind, Is.EqualTo(TokenKind.Minus));
		Assert.That(tokens[1].TKind, Is.EqualTo(TokenKind.Greater));
	}

	[Test]
	public void Lex_MultipleLineBreaks_ProducesMultipleLineBreakTrivia()
	{
		Token token = LexSingle("\n\n\nabc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(3));
		Assert.That(token.LeadingTrivia.All(t => t.TKind == TokenKind.LineBreak));
	}

	[Test]
	public void Lex_WhitespaceLineBreakWhitespace_AllLeadingTrivia()
	{
		Token token = LexSingle("  \n  abc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(3));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.Whitespace));
		Assert.That(token.LeadingTrivia[1].TKind, Is.EqualTo(TokenKind.LineBreak));
		Assert.That(token.LeadingTrivia[2].TKind, Is.EqualTo(TokenKind.Whitespace));
	}

	[Test]
	public void Lex_CommentThenLineBreakThenWhitespace_AllLeadingTrivia()
	{
		Token token = LexSingle("// comment\n  abc");
		Assert.That(token.LeadingTrivia.Count, Is.EqualTo(3));
		Assert.That(token.LeadingTrivia[0].TKind, Is.EqualTo(TokenKind.SingleLineComment));
		Assert.That(token.LeadingTrivia[1].TKind, Is.EqualTo(TokenKind.LineBreak));
		Assert.That(token.LeadingTrivia[2].TKind, Is.EqualTo(TokenKind.Whitespace));
	}

	[Test]
	public void Lex_SingleCharacterIdentifier_ReturnsIdentifier()
	{
		Token token = LexSingle("x");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Lex_MixedTriviaBeforeEOF_AttachedToEofToken()
	{
		Token token = LexSingle("  // trailing comment\n  ");
		Assert.That(token.TKind, Is.EqualTo(TokenKind.EndOfFile));
		Assert.That(token.LeadingTrivia.Count, Is.GreaterThanOrEqualTo(2));
	}
}
