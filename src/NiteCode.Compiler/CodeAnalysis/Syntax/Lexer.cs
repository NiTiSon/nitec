using Microsoft.Extensions.Primitives;
using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

internal sealed class Lexer
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

	[DebuggerStepThrough]
	private char Peek(int offset)
	{
		int index = (int)pos + offset;

		if (index >= text.Length)
			return '\0';

		return text.Text[index];
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

		kind = SyntaxKind.EndOfLineTrivia;
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

	public SyntaxToken NextToken()
	{
		ReadTrivia(leading: true);

		ImmutableArray<SyntaxTrivia> leadingTrivia = triviaBuilder.ToImmutable();
		uint tokenStart = pos;

		ReadToken();

		SyntaxKind tokenKind = kind;
		uint tokenLength = pos - start;

		ReadTrivia(leading: false);

		ImmutableArray<SyntaxTrivia> trailingTrivia = triviaBuilder.ToImmutable();

		StringSegment tokenText = text.ToString(tokenStart, tokenLength);

		return new SyntaxToken(tree, tokenKind, tokenStart, tokenText, leadingTrivia, trailingTrivia);
	}

	private void ReadToken()
	{
		start = pos;
		kind = SyntaxKind.BadToken;

		switch (Current)
		{
			case '\0':
				if (pos >= text.Length)
					kind = SyntaxKind.EndOfFileToken;
				else
				{
					TextSpan span = new(start, 1);
					TextLocation location = new(text, span);
					pos++;
					diagnostics.ReportZeroByte(location);
					kind = SyntaxKind.BadToken;
				}

				break;
			case >= '0' and <= '9' /* or '.' */: // Temporary remove for dot token availability 
				ReadNumericLiteral();
				kind = SyntaxKind.NumericLiteralToken;
				break;
			case '.': ReadTwoCharToken('.', SyntaxKind.DotDotToken, SyntaxKind.DotToken);  break;
			case ':': ReadTwoCharToken(':', SyntaxKind.ColonToken, SyntaxKind.ColonColonToken);  break;
			
			case '*': ReadOneCharOrAssignmentToken(SyntaxKind.AsteriskToken, SyntaxKind.AsteriskEqualsToken); break;
			case '/': ReadOneCharOrAssignmentToken(SyntaxKind.SlashToken, SyntaxKind.SlashEqualsToken); break;
			case '!': ReadOneCharOrAssignmentToken(SyntaxKind.ExclamationToken, SyntaxKind.ExclamationEqualsToken); break;
			case '=': ReadOneCharOrAssignmentToken(SyntaxKind.EqualsToken, SyntaxKind.EqualsEqualsToken); break;
			case '%': ReadOneCharOrAssignmentToken(SyntaxKind.PercentToken, SyntaxKind.PercentEqualsToken); break;
			
			case '+': ReadTwoCharOrAssignmentToken('+', SyntaxKind.PlusPlusToken, SyntaxKind.PlusToken, SyntaxKind.PlusEqualsToken); break;
			case '-': ReadTwoCharOrAssignmentToken('-', SyntaxKind.MinusMinusToken, SyntaxKind.MinusToken, SyntaxKind.MinusEqualsToken); break;
			case '&': ReadTwoCharOrAssignmentToken('&', SyntaxKind.AmpersandAmpersandToken, SyntaxKind.AmpersandToken, SyntaxKind.AmpersandEqualsToken); break;
			case '|': ReadTwoCharOrAssignmentToken('|', SyntaxKind.BarBarToken, SyntaxKind.BarToken, SyntaxKind.BarEqualsToken); break;
			
			case '?': ReadOneCharToken(SyntaxKind.QuestionToken); break;
			case '^': ReadOneCharToken(SyntaxKind.CircumflexToken); break;
			case '(': ReadOneCharToken(SyntaxKind.OpenParenToken); break;
			case ')': ReadOneCharToken(SyntaxKind.CloseParenToken); break;
			case '{': ReadOneCharToken(SyntaxKind.OpenBraceToken); break;
			case '}': ReadOneCharToken(SyntaxKind.CloseBraceToken); break;
			case '[': ReadOneCharToken(SyntaxKind.OpenBracketToken); break;
			case ']': ReadOneCharToken(SyntaxKind.CloseBracketToken); break;
			case ';': ReadOneCharToken(SyntaxKind.SemicolonToken); break;
			default:
				if (IsBeginIdentifier())
				{
					pos++;

					while (IsContinueIdentifier())
						pos++;

					kind = SyntaxKind.IdentifierToken;
					break;
				}

				pos++;
				break;
		}
	}

	private bool TryRead(ReadOnlySpan<char> segment)
	{
		if (segment.IsEmpty) throw new ArgumentException(null, nameof(segment));

		if (pos + segment.Length > text.Length)
			return false;

		if (segment.Equals(text.Text.AsSpan().Slice((int)pos, segment.Length), StringComparison.Ordinal))
		{
			pos += (uint)segment.Length;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadTwoCharToken(char second, SyntaxKind ifTwo, SyntaxKind ifOne)
	{
		if (Next == second)
		{
			kind = ifTwo;
			pos += 2;
		}
		else
		{
			kind = ifOne; 
			pos++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadOneCharToken(SyntaxKind kind)
	{
		pos++;
		this.kind = kind;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadOneCharOrAssignmentToken(SyntaxKind kind, SyntaxKind assignmentKind)
	{
		if (Next is '=')
		{
			pos += 2;
			this.kind = assignmentKind;
		}
		else
		{
			pos++;
			this.kind = kind;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadTwoCharOrAssignmentToken(char second, SyntaxKind ifTwo, SyntaxKind ifOne, SyntaxKind ifAssignmentKind)
	{
		if (Next == second)
		{
			kind = ifTwo;
			pos += 2;
		}
		else if (Next is '=')
		{
			kind = ifAssignmentKind;
			pos += 2;
		}
		else
		{
			kind = ifOne;
			pos++;
		}
	}

	private void ReadNumericLiteral()
	{
		NumericLiteralStyle style = default;

		if (Current is '0') // Try to read prelude
		{
			switch (Next)
			{
				case 'x':
				case 'X':
					style = NumericLiteralStyle.Hexadecimal;
					pos += 2;
					break;
				case 'b':
				case 'B':
					style = NumericLiteralStyle.Binary;
					pos += 2;
					break;
			}
		}

		if (style is NumericLiteralStyle.Hexadecimal)
			while (IsHexadecimal() || Current is '_')
				pos++;
		else if (style is NumericLiteralStyle.Binary)
			while (IsBinary() || Current is '_')
				pos++;
		else // Default may be as integer type, and as float-point type
		{
			while (IsDecimal() || Current is '_')
				pos++;

			bool isFloat = false;

			if (Current is '.') // Floating point
			{
				pos++;
				if (!(IsDecimal() || Current is '_')) // To separate the dot and dotdot tokens from float literal
				{
					pos--;
				}
				else
				{
					while (IsDecimal() || Current is '_')
						pos++;

					isFloat = true;
				}
			}

			// E-notation
			if (Current is 'e' or 'E')
			{
				pos++;
				if (Current is '+' or '-') // Positive or negative (not required)
					pos++;

				while (IsDecimal() || Current is '_')
					pos++;

				isFloat = true;
			}

			if (isFloat)
				TryReadFloatType();
			else
				TryReadIntegerType();

			return;
		}

		TryReadIntegerType();
		return;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsDecimal()
	{
		return Current is >= '0' and <= '9';
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsBinary()
	{
		return Current is '0' or '1';
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsHexadecimal()
	{
		return Current is >= '0' and <= '9'
				or >= 'a' and <= 'f'
				or >= 'A' and <= 'F'
				;
	}

	private bool TryReadNumericType()
	{
		return TryReadIntegerType() || TryReadFloatType();
	}

	private bool TryReadIntegerType()
	{
		return TryRead("u8")
			|| TryRead("u16")
			|| TryRead("u32")
			|| TryRead("u64")
			|| TryRead("i8")
			|| TryRead("i16")
			|| TryRead("i32")
			|| TryRead("i64")
			;
	}
	
	private bool TryReadFloatType()
	{
		return TryRead("f32")
			|| TryRead("f64")
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsBeginIdentifier()
	{
		return char.GetUnicodeCategory(Current)
			is UnicodeCategory.UppercaseLetter
			or UnicodeCategory.LowercaseLetter
			||
			Current is '_' or '@'
			;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsContinueIdentifier()
	{
		return char.GetUnicodeCategory(Current)
			is UnicodeCategory.UppercaseLetter
			or UnicodeCategory.LowercaseLetter
			or UnicodeCategory.DecimalDigitNumber
			||
			Current is '_'
			;
	}

	private enum NumericLiteralStyle
	{
		Default,
		Hexadecimal,
		Binary,
	}
}