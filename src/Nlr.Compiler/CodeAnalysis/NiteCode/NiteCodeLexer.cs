using Microsoft.Extensions.Primitives;
using Nlr.Compiler.Text;
using System;
using System.Collections.Immutable;
using System.Text;
using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

internal class NiteCodeLexer : Lexer
{
	private readonly ImmutableArray<SyntaxTrivia>.Builder triviaBuilder;

	public NiteCodeLexer(SourceText source, DiagnosticBag diagnostics) : base(source, diagnostics)
	{
		triviaBuilder = ImmutableArray.CreateBuilder<SyntaxTrivia>();
	}

	public SyntaxToken Lex()
	{
		if (window.IsAtTheEnd())
		{
			return new SyntaxToken(SyntaxKind.EndOfFile, StringSegment.Empty, window.GetSpan(), [], []);
		}

		return NextToken();
	}

	private SyntaxToken NextToken()
	{
		LexTrivia(isTrailing: false);
		ImmutableArray<SyntaxTrivia> leadingTrivia = triviaBuilder.DrainToImmutable();

		TokenInfo tokenInfo = default;

		window.Start();
		ScanSyntaxToken(ref tokenInfo);
		TextSpan tokenSpan = window.GetSpan();

		LexTrivia(isTrailing: true);
		ImmutableArray<SyntaxTrivia> trailingTrivia = triviaBuilder.DrainToImmutable();

		return Create(tokenInfo, leadingTrivia, trailingTrivia, tokenSpan);
	}

	private void ScanSyntaxToken(ref TokenInfo tokenInfo)
	{
		char ch = window.Peek();
		switch (ch)
		{
			case '.':
				window.Advance();
				tokenInfo.Kind = window.TryAdvance('.') ? SyntaxKind.DotDotToken : SyntaxKind.DotToken;
				break;
			case ':':
				window.Advance();
				tokenInfo.Kind = window.TryAdvance(':') ? SyntaxKind.ColonColonToken : SyntaxKind.ColonToken;
				break;
			case ';':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.SemicolonToken;
				break;
			case ',':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.CommaToken;
				break;
			case '(':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.OpenParenToken;
				break;
			case ')':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.CloseParenToken;
				break;
			case '{':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.OpenBraceToken;
				break;
			case '}':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.CloseBraceToken;
				break;
			case '[':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.OpenBracketToken;
				break;
			case ']':
				window.Advance();
				tokenInfo.Kind = SyntaxKind.CloseBracketToken;
				break;
			case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'):
				if (!ScanIdentifierOrKeyword(ref tokenInfo))
				{
					goto default;
				}
				break;
			case (>= '0' and <= '9'):
				ScanNumericLiteral(ref tokenInfo);
				break;
			case '"':
				ScanStringLiteral(ref tokenInfo);
				break;
			//case '\'':
			//	ScanCharacterLiteral(ref tokenInfo);
			//	break;
			default:
				if (SyntaxFacts.IsIdentifierBeginCharacter(ch))
				{
					ScanIdentifierOrKeyword(ref tokenInfo);
					break;
				}

				tokenInfo.Kind = SyntaxKind.BadToken;
				Report(UnknownSymbol, new(window.Position, 1));
				window.Advance();
				tokenInfo.Text = StringSegment.Empty;
				break;
		}
	}

	private void ScanNumericLiteral(ref TokenInfo tokenInfo)
	{
		tokenInfo.Kind = SyntaxKind.NumericLiteralToken;

		if (window.Peek() == '0' && window.Peek(1) == 'x')
		{
			window.Advance(2);
			// Hex encoding
			while (SyntaxFacts.IsHexDigit(window.Peek()))
			{
				window.Advance();
			}
		}
		else if (window.Peek() == '0' && window.Peek(1) == 'b')
		{
			window.Advance(2);
			// Binary encoding
			while (SyntaxFacts.IsHexDigit(window.Peek()))
			{
				window.Advance();
			}
		}
		else
		{
			while (SyntaxFacts.IsDecimalDigit(window.Peek()))
			{
				window.Advance();
			}
		}

		// TODO: Read suffix (u32, i8, f32, etc.)
	}

	private void ScanStringLiteral(ref TokenInfo tokenInfo)
	{
		window.Advance();

		if (window.Peek() == '"' && window.Peek(1) == '"') // Empty string fast exit
		{
			tokenInfo.Kind = SyntaxKind.CharacterLiteralToken;
			tokenInfo.StringValue = string.Empty;
			return;
		}

		StringBuilder sb = new(); // TODO: optimize
		while (true)
		{
			char ch = window.Peek();

			tokenInfo.Kind = SyntaxKind.StringLiteralToken;
			if (ch == '\\')
			{
				sb.Append(SyntaxFacts.GetEscaped(window.Peek(1)));
				window.Advance();
				continue;
			}
			else if (ch == '"' && window.Peek(-1) != '\\')
			{
				tokenInfo.StringValue = sb.ToString();
				window.Advance();
				break;
			}
			else if (SyntaxFacts.IsNewLineCharacter(ch))
			{
				// GOTO: Diagnostic: not terminated string literal
				break;
			}
			else if (ch == TextWindow.InvalidCharacter) // End of file
			{
				// GOTO: Diagnostic: not terminated string literal
				tokenInfo.StringValue = sb.ToString();
				break;
			}

			sb.Append(ch);
			window.Advance();
		}
	}

	private bool ScanIdentifierOrKeyword(ref TokenInfo tokenInfo)
	{
		if (ScanIdentifier(ref tokenInfo))
		{
			SyntaxKind kind = SyntaxFacts.GetKeywordKind(window.GetText().Value!);

			if (kind == SyntaxKind.None) // Token is an identifier
			{
				tokenInfo.Kind = SyntaxKind.IdentifierToken;
				tokenInfo.Text = window.GetText();
			}
			else if (SyntaxFacts.IsContextualKeyword(kind)) // Keyword may be contextual
			{
				tokenInfo.Text = window.GetText();
				throw new NotImplementedException(); // Contextual keywords are not supported by now.
			}
			else // Definitely keyword
			{
				tokenInfo.Kind = kind;
			}

			return true;
		}
		else
		{
			tokenInfo.Kind = SyntaxKind.None;
			return false;
		}
	}

	private bool ScanIdentifier(ref TokenInfo tokenInfo)
	{
		char ch = window.Peek();

		if (SyntaxFacts.IsIdentifierBeginCharacter(ch))
		{
			window.Advance();

			while (SyntaxFacts.IsIdentifierContinueCharacter(window.Peek()))
			{
				window.Advance();
			}

			return true;
		}

		return false;
	}

	private void LexTrivia(bool isTrailing)
	{
		while (true)
		{
			window.Start();
			char ch = window.Peek();

			switch (ch)
			{
				case ' ':
				case '\t':
					triviaBuilder.Add(ScanWhitespace());
					break;

				case '\r':
				case '\n':
					SyntaxTrivia endOfLine = ScanEndOfLine()!.Value;
					triviaBuilder.Add(endOfLine);
					if (isTrailing)
					{
						return;
					}

					break;
				default:
					return;
			}
		}
	}

	private SyntaxTrivia? ScanEndOfLine()
	{
		char ch;
		switch (ch = window.Peek())
		{
			case '\r':
				window.Advance();
				return window.TryAdvance('\n') ? new SyntaxTrivia(SyntaxKind.EndOfLineTrivia, window.GetSpan(), StringSegment.Empty) : new SyntaxTrivia(SyntaxKind.EndOfLineTrivia, window.GetSpan(), StringSegment.Empty);
			case '\n':
				window.Advance();
				return new SyntaxTrivia(SyntaxKind.EndOfLineTrivia, window.GetSpan(), StringSegment.Empty);
			default:
				return null;
		}
	}

	private SyntaxTrivia ScanWhitespace()
	{
	TOP:
		char ch = window.Peek();

		switch (ch)
		{
			case '\t':       // Horizontal tab
			case '\v':       // Vertical Tab
			case '\f':       // Form-feed
			case '\u001A':
				goto case ' ';

			case ' ':
				window.Advance();
				goto TOP;

			case '\r':      // Carriage Return
			case '\n':      // Line-feed
				break;

			default:
				if (ch > 127 && SyntaxFacts.IsWhitespace(ch))
				{
					goto case '\t';
				}

				break;
		}

		return new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, window.GetSpan(), StringSegment.Empty);
	}

	private static SyntaxToken Create(in TokenInfo info, ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia, TextSpan span)
	{
		SyntaxToken token;
		switch (info.Kind)
		{
			case SyntaxKind.IdentifierToken:
				token = new(info.Kind, info.Text, span, leadingTrivia, trailingTrivia);
				break;
			case SyntaxKind.StringLiteralToken:
				token = new SyntaxTokenWithValue<string>(info.Kind, info.Text, span, info.StringValue, leadingTrivia, trailingTrivia);
				break;
			case SyntaxKind.CharacterLiteralToken:
				token = new SyntaxTokenWithValue<byte>(info.Kind, info.Text, span, info.CharValue, leadingTrivia, trailingTrivia);
				break;
			case SyntaxKind.NumericLiteralToken:
			default:
				token = new SyntaxTokenWithValue<ulong>(info.Kind, StringSegment.Empty, span, 0, leadingTrivia, trailingTrivia);
				break;
		}

		return token;
	}
}