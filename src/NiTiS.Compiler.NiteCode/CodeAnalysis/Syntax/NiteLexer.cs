using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Wasm;
using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed partial class NiteLexer : Lexer
{
	public NiteLexer(DiagnosticBag diagnostics, SourceText sourceText) : base(diagnostics, sourceText) {}

	private struct TokenInfo
	{
		public SyntaxKind Kind;
		public SyntaxKind ContextualKind;
		public TextSpan Span;
		public string? Value;
		public PackedNumeric Number;
		public NumericLiteralType NumberType;
	}

	public override NiteToken Lex()
	{
		ReadTrivia(leading: true);
		ImmutableArray<Trivia> leading = TriviaBuilder.ToImmutable();

		Unsafe.SkipInit(out TokenInfo info);
		ReadToken(ref info);
		info.Span = Window.LexemeSpan;

		ReadTrivia(leading: false);
		ImmutableArray<Trivia> trailing = TriviaBuilder.ToImmutable();

		bool isContextual = false;
		if (info.Kind == SyntaxKind.Identifier)
		{
			info.Kind = SyntaxKindFacts.GetKind(info.Value!);

			if (info.Kind.IsContextual)
			{
				isContextual = true;
				info.ContextualKind = info.Kind;
				info.Kind = SyntaxKind.Identifier;
			}
		}

		if (isContextual)
		{
			return new NiteToken(info.Kind, info.ContextualKind, info.Span, leading, trailing);
		}

		return Create(info, leading, trailing);
	}

	private NiteToken Create(in TokenInfo info, ImmutableArray<Trivia> leading, ImmutableArray<Trivia> trailing)
	{
		if (info.Kind == SyntaxKind.Identifier)
		{
			return new NiteIdentifierToken(info.Value!, info.Span, leading, trailing);
		}

		return new NiteToken(info.Kind, info.Span, leading, trailing);
	}

	private void ReadToken(ref TokenInfo info)
	{
		if (Window.IsAtTheEnd)
		{
			info.Kind = SyntaxKind.EndOfFile;
			return;
		}

		char c = Window.Current;

		if (char.IsAsciiLetter(c))
		{
			ReadIdentifier(skipFirst: true, ref info);
			info.Kind = SyntaxKind.Identifier;
			return;
		}

		switch (c)
		{
			case '+':
				info.Kind = SyntaxKind.PlusToken;
				Window.Advance();
				break;
			case '.':
				Window.Advance();
				if (Window.Current == '.')
				{
					Window.Advance();
					if (Window.Current == '=')
					{
						Window.Advance();
						info.Kind = SyntaxKind.DotDotEqualsToken;
					}
					else
					{
						info.Kind = SyntaxKind.DotDotToken;
					}
					break;
				}
				info.Kind = SyntaxKind.DotToken;
				break;
			case ':':
				Window.Advance();
				info.Kind = Window.AdvanceIfPresented(':') ? SyntaxKind.ColonColonToken : SyntaxKind.ColonToken;
				break;
			case ';':
				Window.Advance();
				info.Kind = SyntaxKind.SemicolonToken;
				break;
			case ',':
				Window.Advance();
				info.Kind = SyntaxKind.CommaToken;
				break;
			case '(':
				Window.Advance();
				info.Kind = SyntaxKind.OpenParenToken;
				break;
			case ')':
				Window.Advance();
				info.Kind = SyntaxKind.CloseParenToken;
				break;
			case '{':
				Window.Advance();
				info.Kind = SyntaxKind.OpenBraceToken;
				break;
			case '}':
				Window.Advance();
				info.Kind = SyntaxKind.CloseBraceToken;
				break;
			case '[':
				Window.Advance();
				info.Kind = SyntaxKind.OpenBracketToken;
				break;
			case ']':
				Window.Advance();
				info.Kind = SyntaxKind.CloseBracketToken;
				break;
			case '#':
				Window.Advance();
				info.Kind = SyntaxKind.HashToken;
				break;
			default:
				if (char.IsLetter(c))
				{
					ReadIdentifier(skipFirst: true, ref info);
					info.Kind = SyntaxKind.Identifier;
					return;
				}
				info.Kind = SyntaxKind.Invalid;
				Window.Advance();
				break;
		}
	}

	private void ReadIdentifier(bool skipFirst, ref TokenInfo info)
	{
		int length = 0;
		if (skipFirst || char.IsLetter(Window.Current))
		{
			length++;
			while (char.IsLetterOrDigit(Window.Peek(length)))
			{
				length++;
			}
		}

		Window.Advance(length);
		info.Kind = SyntaxKind.Identifier;
		info.Value = Window.GetText();
	}

	private void ReadEscapedIdentifier(ref TokenInfo info)
	{
		if (Window.Current == '`')
		{

		}
		throw new NotImplementedException();
	}
}