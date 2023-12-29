using Microsoft.Extensions.Primitives;
using NiteCode.Compiler.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

internal sealed partial class Lexer
{
	private readonly SyntaxTree tree;
	private readonly SourceText text;
	private readonly DiagnosticBag diagnostics;
	
	private uint pos;

	private uint start;
	private SyntaxKind kind;

	private ImmutableArray<SyntaxTrivia>.Builder triviaBuilder = ImmutableArray.CreateBuilder<SyntaxTrivia>();

	public DiagnosticBag Diagnostics
		=> diagnostics;

	public Lexer(SyntaxTree syntaxTree)
	{
		tree = syntaxTree;
		text = syntaxTree.Text;

		diagnostics = [];
	}

	private char Current => Peek(0);
	private char Next => Peek(1);

	private char Peek(int offset)
	{
		int index = (int)pos + offset;

		if (index >= text.Length)
			return '\0';

		return text.Text[index];
	}

	public SyntaxToken NextToken()
	{
		//Console.WriteLine($"Read token @{pos}");
		ReadTrivia(leading: true);

		ImmutableArray<SyntaxTrivia> leadingTrivia = triviaBuilder.ToImmutable();
		uint tokenStart = pos;

		ReadToken();

		SyntaxKind tokenKind = kind;
		uint tokenLength = pos - start;

		ReadTrivia(leading: false);

		ImmutableArray<SyntaxTrivia> trailingTrivia = triviaBuilder.ToImmutable();

		StringSegment tokenText;
		if (tokenKind.IsSymbol())
			tokenText = literals[tokenKind];
		else
			tokenText = text.ToString(tokenStart, tokenLength);

		return new SyntaxToken(tree, tokenKind, tokenStart, tokenText, leadingTrivia, trailingTrivia);
	}

	[DebuggerStepThrough]
	private void ReadTrivia(bool leading)
	{
		triviaBuilder.Clear();
		bool done = false;

		while (!done)
		{
			start = pos;
			kind = SyntaxKind.BadToken;

			switch (Current)
			{
				case '\0':
					done = true;
					break;
				case '/':
					if (Next == '/')
					{
						ReadSingleLineComment();
					}
					else if (Next == '*')
					{
						ReadMultiLineComment();
					}
					else
					{
						done = true;
					}
					break;
				case '\n':
				case '\r':
					if (!leading)
						done = true;
					ReadLineBreak();
					break;
				case ' ':
				case '\t':
					ReadWhiteSpace();
					break;
				default:
					if (char.GetUnicodeCategory(Current) == System.Globalization.UnicodeCategory.SpaceSeparator)
						ReadWhiteSpace();
					else
						done = true;
					break;
			}
		}
	}

	[DebuggerStepThrough]
	private void ReadWhiteSpace()
	{
		while (char.GetUnicodeCategory(Current) == UnicodeCategory.SpaceSeparator || Current is '\t')
		{
			pos++;
		}

		kind = SyntaxKind.WhitespaceTrivia;
	}

	[DebuggerStepThrough]
	private void ReadLineBreak()
	{
		if (Current == '\r' && Next == '\n')
		{
			pos += 2;
		}
		else
		{
			pos++;
		}

		kind = SyntaxKind.LineBreakTrivia;
	}

	[DebuggerStepThrough]
	private void ReadMultiLineComment()
	{
		pos += 2;
		bool done = false;

		while (!done)
		{
			switch (Current)
			{
				case '\0':
					TextSpan span = new(start, 2);
					TextLocation location = new(text, span);

					diagnostics.ReportUnterminatedMultiLineComment(location);
					done = true;
					break;
				case '*':
					if (Next == '/')
					{
						pos++;
						done = true;
					}
					pos++;
					break;
				default:
					pos++;
					break;
			}
		}

		kind = SyntaxKind.MultiLineCommentTrivia;
	}

	[DebuggerStepThrough]
	private void ReadSingleLineComment()
	{
		pos += 2;
		bool done = false;

		while (!done)
		{
			switch (Current)
			{
				case '\0':
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					pos++;
					break;
			}
		}

		kind = SyntaxKind.SingleLineCommentTrivia;
	}

	private void ReadToken()
	{
		start = pos;
		kind = SyntaxKind.BadToken;

		switch (Current)
		{
			case '\0':
				kind = SyntaxKind.EndOfFile;
				break;
			case '+':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.PlusEquals;
					pos++;
				}
				else if (Current == '+')
				{
					kind = SyntaxKind.PlusPlus;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Plus;
				}
				break;
			case '-':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.MinusEquals;
					pos++;
				}
				else if (Current == '-')
				{
					kind = SyntaxKind.MinusMinus;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Minus;
				}
				break;
			case '/':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.SlashEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Slash;
				}
				break;
			case '*':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.AsteriskEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Asterisk;
				}
				break;
			case '%':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.ModuloEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Modulo;
				}
				break;
			case '!':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.BangEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Bang;
				}
				break;
			case '~':
				pos++;
				kind = SyntaxKind.Tilde;
				break;
			case '^':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.CircumflexEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Circumflex;
				}
				break;
			case '&':
				pos++;
				if (Current == '&')
				{
					kind = SyntaxKind.AmpersandAmpersand;
					pos++;
				}
				else if (Current == '=')
				{
					kind = SyntaxKind.AmpersandEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Ampersand;
				}
				break;
			case '|':
				pos++;
				if (Current == '|')
				{
					kind = SyntaxKind.BarBar;
					pos++;
				}
				else if (Current == '=')
				{
					kind = SyntaxKind.BarEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Bar;
				}
				break;
			case '<':
				pos++;
				if (Current == '<')
				{
					pos++;
					if (Current == '=')
					{
						kind = SyntaxKind.LessLessEquals;
						pos++;
					}
					else
					{
						kind = SyntaxKind.LessLess;
					}
				}
				else if (Current == '=')
				{
					kind = SyntaxKind.LessEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Less;
				}
				break;
			case '>':
				pos++;
				if (Current == '>')
				{
					pos++;
					if (Current == '=')
					{
						kind = SyntaxKind.MoreMoreEquals;
						pos++;
					}
					else
					{
						kind = SyntaxKind.MoreMore;
					}
				}
				else if (Current == '=')
				{
					kind = SyntaxKind.MoreEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.More;
				}
				break;
			case '=':
				pos++;
				if (Current == '=')
				{
					kind = SyntaxKind.EqualsEquals;
					pos++;
				}
				else
				{
					kind = SyntaxKind.Equals;
				}
				break;
			case >= '0' and <= '9':
				pos++;
				kind = SyntaxKind.Number;
				ReadNumber();
				break;
			case '.': ReadOneCharOfKind(SyntaxKind.Dot); break;
			case ',': ReadOneCharOfKind(SyntaxKind.Comma); break;
			case ':': ReadOneCharOfKind(SyntaxKind.Colon); break;
			case ';': ReadOneCharOfKind(SyntaxKind.Semicolon); break;
			case '(': ReadOneCharOfKind(SyntaxKind.OpenParenthesis); break;
			case ')': ReadOneCharOfKind(SyntaxKind.ClosedParenthesis); break;
			case '{': ReadOneCharOfKind(SyntaxKind.OpenBrace); break;
			case '}': ReadOneCharOfKind(SyntaxKind.ClosedBrace); break;
			case '[': ReadOneCharOfKind(SyntaxKind.OpenBracket); break;
			case ']': ReadOneCharOfKind(SyntaxKind.ClosedBracket); break;
			default:
				if (IsIdentifierBegin(Current))
				{
					pos++;
					while (IsIdentifierNext(Current))
						pos++;

					uint tokenLength = pos - start;
					StringSegment identifierOrKeyword = text.ToString(start, tokenLength);
					if (keywords.TryGetValue(identifierOrKeyword, out SyntaxKind keyword))
					{
						kind = keyword;
					}
					else
					{
						kind = SyntaxKind.Identifier;
					}
					break;
				}
				diagnostics.ReportInvalidCharacter(new TextLocation(text, TextSpan.FromPoint(pos)), Current);
				kind = SyntaxKind.BadToken;
				pos++;
				break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadOneCharOfKind(SyntaxKind kind)
	{
		pos++;
		this.kind = kind;
	}

	private static bool IsIdentifierBegin(char c)
	{
		return char.GetUnicodeCategory(c)
			is UnicodeCategory.LowercaseLetter
			or UnicodeCategory.UppercaseLetter
			or UnicodeCategory.ModifierLetter
			or UnicodeCategory.OtherLetter
			or UnicodeCategory.TitlecaseLetter
			|| c is '_' or '@';
	}
	private static bool IsIdentifierNext(char c)
	{
		return char.GetUnicodeCategory(c)
			is UnicodeCategory.LowercaseLetter
			or UnicodeCategory.UppercaseLetter
			or UnicodeCategory.ModifierLetter
			or UnicodeCategory.OtherLetter
			or UnicodeCategory.TitlecaseLetter
			or UnicodeCategory.DecimalDigitNumber
			|| c is '_' or '@';
	} 

	private void ReadNumber()
	{
		while (Current is <= '9' and >= '0' or '_')
		{
			pos++;
		}
	}
}