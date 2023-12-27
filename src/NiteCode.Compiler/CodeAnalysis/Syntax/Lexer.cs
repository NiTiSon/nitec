using Microsoft.Extensions.Primitives;
using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

internal sealed class Lexer
{
	private readonly SyntaxTree tree;
	private readonly SourceText text;
	
	private uint pos;

	private uint start;
	private SyntaxKind kind;

	private ImmutableArray<SyntaxTrivia>.Builder triviaBuilder = ImmutableArray.CreateBuilder<SyntaxTrivia>();

	public Lexer(SyntaxTree syntaxTree)
	{
		tree = syntaxTree;
		text = syntaxTree.Text;
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
		if (tokenKind.IsLiteralSymbol())
			tokenText = null!;
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

	private void ReadWhiteSpace()
	{
		while (char.GetUnicodeCategory(Current) == UnicodeCategory.SpaceSeparator || Current is '\t')
		{
			pos++;
		}

		kind = SyntaxKind.WhitespaceTrivia;
	}

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

	private void ReadMultiLineComment()
	{
		pos += 2;
		bool done = false;

		while (!done)
		{
			switch (Current)
			{
				case '\0':
					//TextSpan span = new TextSpan(start, 2);
					//TextLocation location = new TextLocation(_text, span);
					//diagnostics.ReportUnterminatedMultiLineComment(location);
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
				kind = SyntaxKind.NumberLiteral;
				ReadNumber();
				break;
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
				kind = SyntaxKind.BadToken;
				pos++;
				break;
		}
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

	private static readonly FrozenDictionary<SyntaxKind, StringSegment> literals;
	private static readonly FrozenDictionary<StringSegment, SyntaxKind> keywords;
	static Lexer()
	{
		Dictionary<SyntaxKind, StringSegment> literals = new()
		{
			[SyntaxKind.NilConst] = "nil",
			[SyntaxKind.TrueConst] = "true",
			[SyntaxKind.FalseConst] = "false",
			
			[SyntaxKind.Plus] = "+",
			[SyntaxKind.PlusPlus] = "++",
			[SyntaxKind.PlusEquals] = "+=",
			[SyntaxKind.Minus] = "-",
			[SyntaxKind.MinusMinus] = "--",
			[SyntaxKind.MinusEquals] = "-=",
			[SyntaxKind.Asterisk] = "*",
			[SyntaxKind.AsteriskEquals] = "*=",
			[SyntaxKind.Slash] = "/",
			[SyntaxKind.SlashEquals] = "/=",
			
			[SyntaxKind.Bang] = "!",
			[SyntaxKind.Equals] = "=",
			[SyntaxKind.EqualsEquals] = "==",
			[SyntaxKind.BangEquals] = "!=",
			[SyntaxKind.Tilde] = "~",
			[SyntaxKind.Circumflex] = "^",
			[SyntaxKind.CircumflexEquals] = "^=",
			[SyntaxKind.CircumflexEquals] = "^=",
			[SyntaxKind.Ampersand] = "&",
			[SyntaxKind.AmpersandAmpersand] = "&&",
			[SyntaxKind.AmpersandEquals] = "&=",
			[SyntaxKind.Bar] = "|",
			[SyntaxKind.BarBar] = "||",
			[SyntaxKind.BarEquals] = "|=",
			[SyntaxKind.Less] = "<",
			[SyntaxKind.LessLess] = "<<",
			[SyntaxKind.LessEquals] = "<=",
			[SyntaxKind.LessLessEquals] = "<<=",
			[SyntaxKind.More] = ">",
			[SyntaxKind.MoreMore] = ">>",
			[SyntaxKind.MoreEquals] = ">=",
			[SyntaxKind.MoreMoreEquals] = ">>=",

			[SyntaxKind.I8] = "i8",
			[SyntaxKind.I16] = "i16",
			[SyntaxKind.I32] = "i32",
			[SyntaxKind.I64] = "i64",
			[SyntaxKind.I128] = "i128",
			[SyntaxKind.U8] = "u8",
			[SyntaxKind.U16] = "u16",
			[SyntaxKind.U32] = "u32",
			[SyntaxKind.U64] = "u64",
			[SyntaxKind.U128] = "u128",
			[SyntaxKind.F16] = "f16",
			[SyntaxKind.F32] = "f32",
			[SyntaxKind.F64] = "f64",
			[SyntaxKind.F128] = "f128",
			[SyntaxKind.Bool] = "bool",
			[SyntaxKind.Void] = "void",

			[SyntaxKind.PublicKeyword] = "public",
			[SyntaxKind.PrivateKeyword] = "private",
			[SyntaxKind.ProtectedKeyword] = "protected",
			[SyntaxKind.NewKeyword] = "new",
			[SyntaxKind.DeleteKeyword] = "delete",
			[SyntaxKind.TypeKeyword] = "type",
			[SyntaxKind.EnumKeyword] = "enum",
			[SyntaxKind.RefKeyword] = "ref",
			[SyntaxKind.ObjectKeyword] = "object",
			[SyntaxKind.LocalKeyword] = "local",
			[SyntaxKind.ConstKeyword] = "const",
			[SyntaxKind.LiteralKeyword] = "literal",
			[SyntaxKind.StaticKeyword] = "static",
			[SyntaxKind.SizeofKeyword] = "sizeof",
			[SyntaxKind.TypeofKeyword] = "typeof",
			[SyntaxKind.DefaultKeyword] = "default",
			[SyntaxKind.OperatorKeyword] = "operator",
			[SyntaxKind.ExternalKeyword] = "external",
			[SyntaxKind.ImplicitKeyword] = "implicit",

			[SyntaxKind.IfKeyword] = "if",
			[SyntaxKind.ElseKeyword] = "else",
			[SyntaxKind.WhileKeyword] = "while",
			[SyntaxKind.DoKeyword] = "do",
			[SyntaxKind.ReturnKeyword] = "return",
		};
		Dictionary<StringSegment, SyntaxKind> keywords = [];

		foreach (SyntaxKind kind in Enum.GetValues<SyntaxKind>())
		{
			if ((kind.IsKeyword() || kind.IsBuiltInType()) && !kind.IsFlag())
				keywords[literals[kind]] = kind;
		}

		Lexer.literals = literals.ToFrozenDictionary();
		Lexer.keywords = keywords.ToFrozenDictionary();
	}
}