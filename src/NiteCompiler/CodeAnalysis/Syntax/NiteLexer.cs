using NiteCompiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteLexer
{
	private readonly DiagnosticBag _diagnostics;
	private readonly SlidingWindow _window;

	public NiteLexer(SourceText source, DiagnosticBag diagnostics)
	{
		_diagnostics = diagnostics;
		_window = new(source);
	}

	internal ref struct TokenInfo
	{
		public SyntaxKind Kind;
		public SyntaxKind ContextualKind;
	}

	public Token Lex()
	{
		TokenInfo info = default;

		ReadTrivia(true);

		_window.Start();
		ReadToken(ref info);
		TextSpan span = _window.LexemeSpan;
		string? text = info.Kind
			is SyntaxKind.IdentifierToken
			or SyntaxKind.NumberToken
			? _window.Lexeme
			: null;

		ReadTrivia(false);

		switch (info.Kind)
		{
			case SyntaxKind.IdentifierToken:
				return new IdentifierToken(info.Kind, info.ContextualKind, span, text!);
			case SyntaxKind.NumberToken:
				return NumericParser.Parse(ref info, text, span, _diagnostics);
			default:
				return new(info.Kind, span);
		}
	}

	private void ReadToken(ref TokenInfo info)
	{
		if (_window.IsAtTheEnd)
		{
			info.Kind = SyntaxKind.EndOfFile;
			return;
		}

		switch (_window.Current)
		{
			case >= 'a' and <= 'z':
			case >= 'A' and <= 'Z':
				_window.Advance();
				ReadIdentifierSkipFirst(ref info);
				break;
			case ':':
				_window.Advance();
				if (_window.Current == ':')
				{
					info.Kind = SyntaxKind.ColonColonToken;
					_window.Advance();
				}
				else
				{
					info.Kind = SyntaxKind.ColonToken;
				}

				break;
			case '?':
				if (_window.Next == '?')
				{
					if (_window.Peek(2) == '=')
					{
						_window.Advance(3);
						info.Kind = SyntaxKind.QuestionQuestionEqualsToken;
					}
					else
					{
						_window.Advance(2);
						info.Kind = SyntaxKind.QuestionQuestionToken;
					}
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.QuestionToken;
				}

				break;
			case '-':
				if (_window.Next == '>')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.RetusaToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.MinusToken;
				}

				break;
			case '+':
				_window.Advance();
				info.Kind = SyntaxKind.PlusToken;
				break;
			case '*':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.AsteriskEqualsToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.AsteriskToken;
				}

				break;
			case '/':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.SlashEqualsToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.SlashToken;
				}

				break;
			case '&':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.AmpersandEqualsToken;
				}
				else if (_window.Next == '&')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.AmpersandAmpersandToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.AmpersandToken;
				}

				break;
			case '!':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.ExclamationEqualsToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.ExclamationToken;
				}

				break;
			case '%':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.PercentEqualsToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.PercentToken;
				}

				break;
			case '|':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.PipeEqualsToken;
				}
				else if (_window.Next == '|')
				{

					_window.Advance(2);
					info.Kind = SyntaxKind.PipePipeToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.PipeToken;
				}

				break;
			case '^':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.CaretEqualsToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.CaretToken;
				}

				break;
			case '=':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.EqualsEqualsToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.EqualsToken;
				}

				break;
			case '<':
				if (_window.Next == '<')
				{
					if (_window.Peek(2) == '=') // <<=
					{
						_window.Advance(3);
						info.Kind = SyntaxKind.LeftShiftEqualsToken;
					}
					else
					{
						_window.Advance(2);
						info.Kind = SyntaxKind.LeftShiftToken;
					}
				}
				else if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = SyntaxKind.LessThanEqualsToken;
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.LessThanToken;
				}

				break;
			case '>':
				// Lexer can't distinct >> (right shift) and >> (two generic list terminators) out of context
				// so we do it on Parser 🤣

				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind =  SyntaxKind.GreaterThanEqualsToken;
				}
				else
				{
					_window.Advance(1);
					info.Kind = SyntaxKind.GreaterThanToken;
				}
				break;
			case ';':
				_window.Advance();
				info.Kind = SyntaxKind.SemicolonToken;
				break;
			case '{':
				_window.Advance();
				info.Kind = SyntaxKind.OpenBraceToken;
				break;
			case '}':
				_window.Advance();
				info.Kind = SyntaxKind.CloseBraceToken;
				break;
			case '(':
				_window.Advance();
				info.Kind = SyntaxKind.OpenParenToken;
				break;
			case ')':
				_window.Advance();
				info.Kind = SyntaxKind.CloseParenToken;
				break;
			case ',':
				_window.Advance();
				info.Kind = SyntaxKind.CommaToken;
				break;
			case '.':
				if (_window.Next == '.')
				{
					if (_window.Peek(2) == '=')
					{
						_window.Advance(3);
						info.Kind = SyntaxKind.DotDotEqualsToken;
					}
					else
					{
						_window.Advance(2);
						info.Kind = SyntaxKind.DotDotToken;
					}
				}
				else
				{
					_window.Advance();
					info.Kind = SyntaxKind.DotToken;
				}

				break;
			case >= '0' and <= '9':
				info.Kind = SyntaxKind.NumberToken;
				_window.Advance();
				while (char.IsAsciiDigit(_window.Current)) _window.Advance();
				break;
			default:
				ReadIdentifier(ref info);

				if (_window.Width == 0)
				{
					_window.Advance();
					info.Kind = SyntaxKind.Invalid;
				}

				break;
		}

		if (info.Kind == SyntaxKind.IdentifierToken && SyntaxFacts.IsPossibleKeyword(_window.Width))
			SyntaxFacts.DefineKeywordOrIdentifier(_window.Lexeme, ref info);
	}
}