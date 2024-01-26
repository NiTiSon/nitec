using Microsoft.Extensions.Primitives;
using NiteCode.Compiler.CodeAnalysis.Text;
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
				kind = SyntaxKind.EndOfFileToken;
				break;
			case >= '0' and <= '9':
				pos++;
				while(Current is >= '0' and <= '9')
				{
					pos++;
				}
				kind = SyntaxKind.NumericLiteralToken;
				break;
			case '(': ReadOneCharToken(SyntaxKind.OpenParenToken); break;
			case ')': ReadOneCharToken(SyntaxKind.CloseParenToken); break;
			case '{': ReadOneCharToken(SyntaxKind.OpenBraceToken); break;
			case '}': ReadOneCharToken(SyntaxKind.CloseBraceToken); break;
			case '[': ReadOneCharToken(SyntaxKind.OpenBracketToken); break;
			case ']': ReadOneCharToken(SyntaxKind.CloseBracketToken); break;
			case '+': ReadOneCharToken(SyntaxKind.PlusToken); break;
			case '-': ReadOneCharToken(SyntaxKind.MinusToken); break;
			case '*': ReadOneCharToken(SyntaxKind.AsteriskToken); break;
			case '/': ReadOneCharToken(SyntaxKind.SlashToken); break;
			case '%': ReadOneCharToken(SyntaxKind.PercentToken); break;
			default:
				pos++;
				break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadOneCharToken(SyntaxKind kind)
	{
		pos++;
		this.kind = kind;
	}
}