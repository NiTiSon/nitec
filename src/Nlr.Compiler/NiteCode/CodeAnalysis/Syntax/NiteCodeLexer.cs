using System;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteCodeLexer : Lexer
{
	private SyntaxKind _kind;

	public NiteCodeLexer(DiagnosticBag diagnostics, Source source) : base(diagnostics, source)
	{
	}

	private int _previousTrailingTokens;
	private int _currentLeadingTokens;
	
	private int BeforeTriviaCount => _currentLeadingTokens + _previousTrailingTokens;
	
	public override Token Lex()
	{
		ReadTrivia(leading: true);
		ImmutableArray<Trivia> leadingTrivia = _triviaBuilder.ToImmutable();

		_window.Start();
		ReadToken();
		SyntaxKind kind = _kind;
		
		TextSpan span = new(_window.Position, _window.Width);
		string? text = SyntaxFacts.GetText(_kind);
		
		text ??= _window.GetText();

		if (kind == SyntaxKind.IdentifierToken)
		{
			kind = SyntaxFacts.GetKind(text) ?? kind;
		}
		
		ReadTrivia(leading: false);
		ImmutableArray<Trivia> trailingTrivia = _triviaBuilder.ToImmutable();
		
		return new Token(kind, span, text, leadingTrivia, trailingTrivia);
	}

	private void ReadToken()
	{
		if (_window.IsAtTheEnd)
		{
			_kind = SyntaxKind.EndOfFile;
			return;
		}

		switch (_window.Current)
		{
			case '+':
				switch (_window.Next)
				{
					case '=':
						_kind = SyntaxKind.PlusEqualsToken;
						_window.Advance(2);
						break;
					case '+':
						_kind = SyntaxKind.PlusPlusToken;
						_window.Advance(2);
						break;
					default:
						_kind = SyntaxKind.PlusToken;
						_window.Advance();
						break;
				}
				break;
			case '.':
				if (_window.Next == '.')
				{
					if (_window.Peek(2) == '=')
					{
						_window.Advance(3);
						_kind = SyntaxKind.DotDotEqualsToken;
					}
					else
					{
						_window.Advance(2);
						_kind = SyntaxKind.DotDotToken;
					}
				}
				else
				{
					_window.Advance();
					_kind = SyntaxKind.DotToken;
				}
				break;
			case ',':
				_kind = SyntaxKind.CommaToken;
				_window.Advance();
				break;
			case '~':
				if (_window.Next == '=')
				{
					_kind = SyntaxKind.TildaEqualsToken;
					_window.Advance(2);
				}
				else
				{
					_kind = SyntaxKind.TildaToken;
					_window.Advance();
				}
				break;
			case '!':
				switch (_window.Next)
				{
					case '.':
						_kind = SyntaxKind.ExclamationMarkDotToken;
						_window.Advance(2);
						break;
					case '=':
						_kind = SyntaxKind.ExclamationMarkEqualsToken;
						_window.Advance(2);
						break;
					default:
						_kind = SyntaxKind.ExclamationMarkToken;
						_window.Advance();
						break;
				}
				break;
			case '?':
				switch (_window.Next)
				{
					case '?':
						if (_window.Peek(2) == '=')
						{
							_window.Advance(3);
							
							_kind = SyntaxKind.QuestionMarkQuestionMarkEqualsToken;
							break;
						}
						_window.Advance(2);
						_kind = SyntaxKind.QuestionMarkQuestionMarkToken;
						break;
					case '.':
						_window.Advance(2);
						_kind = SyntaxKind.QuestionMarkDotToken;
						break;
					case '*':
						if (_window.Next == '=')
						{
							_window.Advance(2);
							_kind = SyntaxKind.AsteriskEqualsToken;
						}
						else
						{
							_window.Advance();
							_kind = SyntaxKind.AsteriskToken;
						}
						break;
					default:
						_window.Advance();
						_kind = SyntaxKind.QuestionMarkToken;
						break;
				}
				break;
			case '-':
				switch (_window.Next)
				{
					case '=':
						_kind = SyntaxKind.MinusEqualsToken;
						_window.Advance(2);
						break;
					case '-':
						_kind = SyntaxKind.MinusMinusToken;
						_window.Advance(2);
						break;
					case '>':
						_kind = SyntaxKind.MinusGreaterThanToken;
						_window.Advance(2);
						break;
					default:
						_kind = SyntaxKind.MinusToken;
						_window.Advance();
						break;
				}
				break;
			case '*':
				if (_window.Next == '=')
				{
					_kind = SyntaxKind.AsteriskEqualsToken;
					_window.Advance(2);
				}
				else
				{
					_kind = SyntaxKind.AsteriskToken;
					_window.Advance();
				}
				break;
			case '/':
				if (_window.Next == '=')
				{
					_kind = SyntaxKind.SlashEqualsToken;
					_window.Advance(2);
				}
				else
				{
					_kind = SyntaxKind.SlashToken;
					_window.Advance();
				}
				break;
			case '%':
				if (_window.Next == '=')
				{
					_kind = SyntaxKind.PercentEqualsToken;
					_window.Advance(2);
				}
				else
				{
					_kind = SyntaxKind.PercentToken;
					_window.Advance();
				}
				break;
			case ':':
				if (_window.Next == ':')
				{
					_kind = SyntaxKind.ColonColonToken;
					_window.Advance(2);
				}
				else
				{
					_kind = SyntaxKind.ColonToken;
					_window.Advance();
				}
				break;
			case ';':
				_kind = SyntaxKind.SemicolonToken;
				_window.Advance();
				break;
			case '{':
				_kind = SyntaxKind.OpenBraceToken;
				_window.Advance();
				break;
			case '}':
				_kind = SyntaxKind.CloseBraceToken;
				_window.Advance();
				break;
			case '(':
				_kind = SyntaxKind.OpenParenToken;
				_window.Advance();
				break;
			case ')':
				_kind = SyntaxKind.CloseParenToken;
				_window.Advance();
				break;
			case '[':
				_kind = SyntaxKind.OpenBracketToken;
				_window.Advance();
				break;
			case ']':
				_kind = SyntaxKind.CloseBracketToken;
				_window.Advance();
				break;
			case '>':
				switch (_window.Next)
				{
					case '>' when BeforeTriviaCount != 0: // TODO: Fix >>= is required to be space before 
						if (_window.Peek(2) == '>')
						{
							if (_window.Peek(3) == '=')
							{
								_kind = SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken;
								_window.Advance(4);
							}
							else
							{
								_kind = SyntaxKind.GreaterThanGreaterThanGreaterThanToken;
								_window.Advance(3);
							}
						}
						else if (_window.Peek(2) == '=')
						{
							_kind = SyntaxKind.GreaterThanGreaterThanEqualsToken;
							_window.Advance(3);
						}
						else
						{
							_kind = SyntaxKind.GreaterThanGreaterThanToken;
							_window.Advance(2);
						}
						break;
					case '=':
						_kind = SyntaxKind.GreaterThanEqualsToken;
						_window.Advance(2);
						break;
					default:
						_kind = SyntaxKind.GreaterThanToken;
						_window.Advance();
						break;
				}
				break;
			case '<':
				switch (_window.Next)
				{
					case '<':
						if (_window.Peek(2) == '=')
						{
							_kind = SyntaxKind.LessThanLessThanEqualsToken;
							_window.Advance(3);
						}
						else
						{
							_kind = SyntaxKind.LessThanLessThanToken;
							_window.Advance(2);
						}
						break;
					case '=':
						_kind = SyntaxKind.LessThanEqualsToken;
						_window.Advance(2);
						break;
					default:
						_kind = SyntaxKind.LessThanToken;
						_window.Advance();
						break;
				}
				break;
			case >= '0' and <= '9':
				ReadNumber();
				break;
			default:
				if (char.IsLetter(_window.Current) || _window.Current == '_')
				{
					ReadIdentifierOrKeyword();
					return;
				}
				
				_kind = SyntaxKind.UnknownOrWrong;
				_window.Advance();
				TextSpan span = new(_window.Position, 1);
				Diagnostics.ReportBadToken(span);
				return;
		}
	}

	private void ReadNumber()
	{
		_kind = SyntaxKind.NumberToken;
		if (_window.Current == '0')
		{
			switch (_window.Next)
			{
				case 'x':
					_window.Advance(2);
					ReadNumberX16();
					ReadIntegerPostfix();
					return;
				case 'b':
					_window.Advance(2);
					ReadNumberX2();
					ReadIntegerPostfix();
					return;
			}
		}

		while (char.IsAsciiDigit(_window.Current))
		{
			_window.Advance();
		}

		ReadIntegerPostfix();
	}

	private bool ReadIntegerPostfix()
	{
		return _window.AdvanceIfPresented("i8")
		|| _window.AdvanceIfPresented("i16")
		|| _window.AdvanceIfPresented("i32")
		|| _window.AdvanceIfPresented("i64")
		|| _window.AdvanceIfPresented("u8")
		|| _window.AdvanceIfPresented("u16")
		|| _window.AdvanceIfPresented("u32")
		|| _window.AdvanceIfPresented("u64")
		|| _window.AdvanceIfPresented('u')
		|| _window.AdvanceIfPresented('i')
		;
	}

	private void ReadNumberX16()
	{
		while (_window.Current is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F')
		{
			_window.Advance();
		}
	}
	
	private void ReadNumberX8()
	{
		while (_window.Current is >= '0' and <= '7')
		{
			_window.Advance();
		}
	}

	private void ReadNumberX2()
	{
		while (_window.Current is '0' or '1')
		{
			_window.Advance();
		}
	}

	private void ReadIdentifierOrKeyword()
	{
		while (char.IsLetterOrDigit(_window.Current) || _window.Current == '_')
		{
			_window.Advance();
		}
		
		_kind = SyntaxKind.IdentifierToken;
	}

	private void ReadTrivia(bool leading)
	{
		_triviaBuilder.Clear();

		bool done = false;

		while (!done)
		{
			_window.Start();
			_kind = SyntaxKind.UnknownOrWrong;

			switch (_window.Current)
			{
				case Window.InvalidCharacter:
					done = true;
					break;
				case '/':
					switch (_window.Next)
					{
						case '/':
							ReadSingleLineComment();
							break;
						case '*':
							ReadMultiLineComment();
							break;
						default:
							done = true;
							break;
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
					if (char.IsWhiteSpace(_window.Current))
						ReadWhiteSpace();
					else
						done = true;
					break;
			}

			if (_window.Width <= 0) continue;
			
			string text = _window.GetText();
			Trivia trivia = new Trivia(_kind, _window.LexemeStart, text);
			_triviaBuilder.Add(trivia);
		}

		if (!leading)
		{
			_previousTrailingTokens = _triviaBuilder.Count;
		}
		else
		{
			_currentLeadingTokens = _triviaBuilder.Count;
		}
	}

	private void ReadLineBreak()
	{
		_kind = SyntaxKind.LineBreakTrivia;
		_window.AdvancePastNewLine();
	}
	
	private void ReadWhiteSpace()
	{
		bool done = false;

		while (!done)
		{
			switch (_window.Current)
			{
				case Window.InvalidCharacter:
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					if (!char.IsWhiteSpace(_window.Current))
						done = true;
					else
						_window.Advance();
					break;
			}
		}

		_kind = SyntaxKind.WhitespaceTrivia;
	}
	
	private void ReadSingleLineComment()
	{
		_window.Advance(2);
		bool done = false;

		while (!done)
		{
			switch (_window.Current)
			{
				case '\0':
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					_window.Advance();
					break;
			}
		}

		_kind = SyntaxKind.SingleLineCommentTrivia;
	}
	
	private void ReadMultiLineComment()
	{
		_window.Advance(2);
		bool done = false;

		while (!done)
		{
			switch (_window.Current)
			{
				case Window.InvalidCharacter:
					TextSpan span = new(_window.Position, 2);
					Diagnostics.ReportNotTerminatedMultiLineComment(span);
					done = true;
					break;
				case '*':
					if (_window.Next == '/')
					{
						_window.Advance();
						done = true;
					}
					_window.Advance();
					break;
				default:
					_window.Advance();
					break;
			}
		}

		_kind = SyntaxKind.MultiLineCommentTrivia;
	}
}