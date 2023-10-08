using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Syntax;
using NiteCompiler.Analysis.Syntax.Tokens;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis;

public sealed partial class Lexer : TokenStream
{
	/// <summary>
	/// The source text file consumed for tokenization.
	/// </summary>
	private readonly CodeSource source;
	private readonly LanguageOptions options;
	private readonly DiagnosticBag diagnostics;
	private uint pos, line, column;
	private bool isDirective;

	public Lexer(LanguageOptions options, CodeSource source, DiagnosticBag diagnostics)
	{
		this.options = options;
		this.source = source;
		this.diagnostics = diagnostics;
		pos = column = 0;
		line = 1;
		InitialMove();
	}

	public bool IsEOF
		=> pos >= source.Content.Length;
	private TextAnchor Position
		=> new(pos, line, column);
	private StringSegment TextSegment
		=> new(source.Content, (int)beginPos.Index, (int)(pos - beginPos.Index));

	private char c;
	private TextAnchor beginPos;
	public Token NextToken()
	{
	BEGIN:
		if (IsEOF)
			return new EOFToken(Position);


		if (c is '\n' or '\r')
		{
			MoveNext();
			goto BEGIN;
		}

		beginPos = Position;

		//if (isDirective)
		//	return NextDirectiveToken();

		if (IsSpace(c))
		{
			MoveNext();

			while (IsSpace(c))
			{
				MoveNext();
			}

			return new TriviaToken(SyntaxKind.WhitespaceTrivia, beginPos, TextSegment);
		}
		
		if (IsCharBegin(c))
		{
			MoveNext();

			while (IsCharContinue(c))
			{
				MoveNext();
			}

			if (TextSegment == Names.True)
			{
				return new BooleanLiteralToken(true, beginPos, TextSegment);
			}
			else if (TextSegment == Names.False)
			{
				return new BooleanLiteralToken(false, beginPos, TextSegment);
			}

			return new IdentifierOrKeywordToken(GetKind(TextSegment), beginPos, TextSegment);
		}

		if (c == '/')
		{
			MoveNext();
			
			if (c == '/')
			{
				MoveNext();
				while (c is not '\n')
					MoveNext();

				return new TriviaToken(SyntaxKind.SingleLineCommentTrivia, beginPos, TextSegment);
			}
			if (c == '*')
			{
				char prev = '\0';
				MoveNext();
				while (prev != '*' && c != '/' && !IsEOF)
				{
					MoveNext();
				}
				MoveNext();

				return new TriviaToken(SyntaxKind.MultiLineCommentTrivia, beginPos, TextSegment);
			}
			if (c == '=')
			{
				MoveNext();

				return new OperatorToken(SyntaxKind.SlashAssign, beginPos, TextSegment);
			}

			return new OperatorToken(SyntaxKind.Slash, beginPos, TextSegment);
		}

		{ // Operators
			SyntaxKind kind = default;
			if (ConsumeOperator('+', SyntaxKind.Plus, SyntaxKind.PlusAssign, ref kind)
			|| ConsumeOperator('-', SyntaxKind.Minus, SyntaxKind.MinusAssign, ref kind)
			|| ConsumeOperator('*', SyntaxKind.Asterisk, SyntaxKind.AsteriskAssign, ref kind)
			|| ConsumeOperator('^', SyntaxKind.Xor, SyntaxKind.XorAssign, ref kind)
			|| ConsumeOperator('=', SyntaxKind.Assign, SyntaxKind.Equals, ref kind)
			|| ConsumeOperator('!', SyntaxKind.Exclamation, SyntaxKind.NotEquals, ref kind)
			|| ConsumeOperator('<', SyntaxKind.Less, SyntaxKind.LessEquals, ref kind)
			|| ConsumeOperator('>', SyntaxKind.More, SyntaxKind.MoreEquals, ref kind)
			)
			{
					return new OperatorToken(kind, beginPos, TextSegment);
				}
			else
			{
				if (c == '|')
				{
					MoveNext();
					if (c == '|')
					{
						return new OperatorToken(SyntaxKind.FastOr, beginPos, TextSegment);
					}
					else if (c == '=')
					{
						return new OperatorToken(SyntaxKind.OrAssign, beginPos, TextSegment);
					}

					return new OperatorToken(SyntaxKind.Or, beginPos, TextSegment);
				}
				if (c == '&')
				{
					MoveNext();
					if (c == '&')
					{
						return new OperatorToken(SyntaxKind.FastAnd, beginPos, TextSegment);
					}
					else if (c == '=')
					{
						return new OperatorToken(SyntaxKind.AndAssign, beginPos, TextSegment);
					}

					return new OperatorToken(SyntaxKind.And, beginPos, TextSegment);
				}
			}
		}

		if (c == '{')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.OpenBrace, beginPos, TextSegment);
		}
		if (c == '}')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.CloseBrace, beginPos, TextSegment);
		}
		if (c == '(')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.OpenParen, beginPos, TextSegment);
		}
		if (c == ')')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.CloseParen, beginPos, TextSegment);
		}
		if (c == '[')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.OpenBracket, beginPos, TextSegment);
		}
		if (c == ']')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.CloseBracket, beginPos, TextSegment);
		}
		if (c == ',')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.Comma, beginPos, TextSegment);
		}
		if (c == '.')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.Dot, beginPos, TextSegment);
		}
		if (c == ':')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.Colon, beginPos, TextSegment);
		}
		if (c == ';')
		{
			MoveNext();
			return new PunctuationToken(SyntaxKind.Semicolon, beginPos, TextSegment);
		}

		if (c == '"')
		{
			char prev;

		NEXT_CHAR:
			prev = c;
			MoveNext();
			
			if ((c == '"' && prev != '\\') || IsEOF)
			{
				MoveNext();


				StringLiteralKind stringKind;
				if (Consume(Names.U8)) stringKind = StringLiteralKind.U8;
				else if (Consume(Names.U16)) stringKind = StringLiteralKind.U16;
				else if (Consume(Names.U32)) stringKind = StringLiteralKind.U32;
				else stringKind = StringLiteralKind.Default;

				return new StringLiteralToken(stringKind, beginPos, TextSegment);
			}

			goto NEXT_CHAR;
		}

		MoveNext();
		while (true)
		{
			if (IsEOF || c is '\n' or '\r' || IsSpace(c))
				break;

			MoveNext();
		}

		diagnostics.ReportError(source, beginPos, ErrorCodes.InvalidCharacterSequence, ErrorMessages.InvalidCharacterSequence);
		return new BadToken(beginPos);
	}

	private bool ConsumeOperator(char operatorSymbol, SyntaxKind operation, SyntaxKind operationAssign, ref SyntaxKind kind)
	{
		if (c == operatorSymbol)
		{
			MoveNext();

			if (ConsumeEquals())
				kind = operationAssign;
			else
				kind = operation;

			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	private SyntaxKind GetKind(StringSegment segment)
	{
		if (segment[0] is '@') // Fast exit for @identifier
			return SyntaxKind.Identifier;

		if (segment == Names.Char)
			return SyntaxKind.KwChar;
		else if (segment == Names.Char8)
			return SyntaxKind.KwChar8;
		else if (segment == Names.Char16)
			return SyntaxKind.KwChar16;
		else if (segment == Names.Char32)
			return SyntaxKind.KwChar32;
		else if (segment == Names.I8)
			return SyntaxKind.KwI8;
		else if (segment == Names.I16)
			return SyntaxKind.KwI16;
		else if (segment == Names.I32)
			return SyntaxKind.KwI32;
		else if (segment == Names.I64)
			return SyntaxKind.KwI64;
		//else if (segment == Names.I128)
		//	return SyntaxKind.KwI128;
		else if (segment == Names.U8)
			return SyntaxKind.KwU8;
		else if (segment == Names.U16)
			return SyntaxKind.KwU16;
		else if (segment == Names.U32)
			return SyntaxKind.KwU32;
		else if (segment == Names.U64)
			return SyntaxKind.KwU64;
		//else if (segment == Names.U128)
		//	return SyntaxKind.KwU128;
		else if (segment == Names.F16)
			return SyntaxKind.KwF16;
		else if (segment == Names.F32)
			return SyntaxKind.KwF32;
		else if (segment == Names.F64)
			return SyntaxKind.KwF64;
		else if (segment == Names.Bool)
			return SyntaxKind.KwBool;
		else if (segment == Names.Void)
			return SyntaxKind.KwVoid;
		else if (segment == Names.Nil)
			return SyntaxKind.KwNil;
		else if (segment == Names.SelfInst)
			return SyntaxKind.KwSelf;
		else if (segment == Names.SelfType)
			return SyntaxKind.KwSelfType;
		else if (segment == Names.SelfType)
			return SyntaxKind.KwSelfType;
		else if (segment == Names.Pubic)
			return SyntaxKind.KwPublic;
		else if (segment == Names.Protected)
			return SyntaxKind.KwProtected;
		else if (segment == Names.Private)
			return SyntaxKind.KwPrivate;
		else if (segment == Names.Homie)
			return SyntaxKind.KwHomie;
		else if (segment == Names.Internal)
			return SyntaxKind.KwInternal;
		else if (segment == Names.Family)
			return SyntaxKind.KwFamily;
		else if (segment == Names.Static)
			return SyntaxKind.KwStatic;
		else if (segment == Names.Virtual)
			return SyntaxKind.KwVirtual;
		else if (segment == Names.Abstract)
			return SyntaxKind.KwAbstract;
		else if (segment == Names.Alias)
			return SyntaxKind.KwAlias;
		else if (segment == Names.Limit)
			return SyntaxKind.KwLimit;
		else if (segment == Names.Type)
			return SyntaxKind.KwType;
		else if (segment == Names.Enum)
			return SyntaxKind.KwEnum;
		else if (segment == Names.Trait)
			return SyntaxKind.KwTrait;
		else if (segment == Names.Ref)
			return SyntaxKind.KwRef;
		else if (segment == Names.Local)
			return SyntaxKind.KwLocal;
		else if (segment == Names.Return)
			return SyntaxKind.KwReturn;
		else if (segment == Names.Continue)
			return SyntaxKind.KwContinue;
		else if (segment == Names.Goto)
			return SyntaxKind.KwGoto;
		else if (segment == Names.Break)
			return SyntaxKind.KwBreak;
		else if (segment == Names.For)
			return SyntaxKind.KwFor;
		else if (segment == Names.If)
			return SyntaxKind.KwIf;
		else if (segment == Names.Else)
			return SyntaxKind.KwElse;
		else if (segment == Names.While)
			return SyntaxKind.KwWhile;
		else if (segment == Names.Do)
			return SyntaxKind.KwDo;
		else if (segment == Names.Loop)
			return SyntaxKind.KwLoop;
		else if (segment == Names.Default)
			return SyntaxKind.KwDefault;
		else if (segment == Names.SizeOf)
			return SyntaxKind.KwSizeOf;
		else if (segment == Names.TypeOf)
			return SyntaxKind.KwTypeOf;

		return SyntaxKind.Identifier;
	}

	private void InitialMove()
	{
		if (IsEOF) return;

		char first = source.Content[0];
		if (first is '\n')
		{
			line++;
			column = 0;
		}
		else if (first is not '\r')
		{
			column++;
		}

		c = first;
	}

	private bool ConsumeEquals()
		=> Consume('=');
	
	private bool Consume(char c)
	{
		if (this.c == c)
		{
			MoveNext();
			return true;
		}

		return false;
	}

	private bool Consume(string s)
	{
		int reqLength = s.Length;

		if (pos + reqLength >  source.Content.Length)
		{
			return false; // Out of range
		}

		if (source.Content.Substring((int)pos, reqLength) == s)
		{
			for (int i = 0; i < reqLength; i++)
				MoveNext();

			return true;
		}
		return false;
	}

	[DebuggerStepThrough]
	private bool MoveNext()
	{
		if (pos >= source.Content.Length - 1)
		{
			#if DEBUG
			pos = (uint)source.Content.Length;
			System.Console.Error.WriteLine($"Stream is end: {pos}");
			#endif
			return false;
		}

		char next = source.Content[(int)++pos];
		if (next is '\n')
		{
			line++;
			column = 0;
			isDirective = false;
		}
		else if (next is not '\r')
		{
			column++;
		}

		c = next;
		return true;
	}
}