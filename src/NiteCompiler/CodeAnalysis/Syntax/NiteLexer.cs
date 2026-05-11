using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal sealed partial class NiteLexer
{
	private readonly DiagnosticBag _diagnostics;
	private readonly SlidingWindow _window;
	private readonly LexerCache _cache;
	private readonly SourceText _source;
	private readonly SyntaxTree _syntaxTree;
	private readonly NiteCompilationOptions _options;
	private Mode _mode = Mode.Syntax;

	private enum Mode
	{
		/// <summary>
		/// Usual syntax.
		/// </summary>
		Syntax,
		/// <summary>
		/// Threat text as Markdown comment.
		/// </summary>
		DocumentationComment,
		/// <summary>
		/// Threat text as non-standard item reference.
		/// <code>
		/// /// This is [[Bar.foo]] item reference.
		/// </code>
		/// </summary>
		DocumentationReference
	}

	private DocumentationMode DocumentationMode => _options.DocumentationMode;

	public NiteLexer(SyntaxTree tree, NiteCompilationOptions options, DiagnosticBag diagnostics)
	{
		_diagnostics = diagnostics;
		_cache = LexerCache.GetInstance();
		_source = tree.Text;
		_window = new(_source);
		_syntaxTree = tree;
		_options = options;
	}

	public Token Lex()
	{
		TokenInfo info = default;

		ReadTrivia(true, _cache.LeadingTrivia);

		_window.Start();
		ReadToken(ref info);
		TextSpan span = _window.LexemeSpan;

		string? text = null;
		if (info.Kind == TokenKind.IdentifierOrKeyword)
		{
			text = _window.Lexeme;
			if (SyntaxFacts.IsPossibleKeyword(_window.Width))
			{
				SyntaxFacts.DefineKeywordOrIdentifier(text, ref info);
			}
		}
		else if (info.Kind == TokenKind.NumberLiteral)
		{
			text = _window.Lexeme;
		}
		else if (info.Kind == TokenKind.EscapedIdentifier)
		{
			text = _cache.StringBuilder.ToString();
		}
		else if (info.Kind == TokenKind.StringLiteral ||
		         info.Kind == TokenKind.CharacterLiteral)
		{
			text = _cache.StringBuilder.ToString();
		}
		else if (info.Kind == TokenKind.LifetimeIdentifier)
		{
			text = _cache.StringBuilder.ToString();
		}

		ReadTrivia(false, _cache.TrailingTrivia);

		var leading = _cache.LeadingTrivia.Build(_syntaxTree);
		var trailing = _cache.TrailingTrivia.Build(_syntaxTree);

		if (info.Kind == TokenKind.IdentifierOrKeyword)
		{
			return new IdentifierOrKeywordToken(_syntaxTree, info.Kind, span, text!, leading, trailing);
		}
		if (info.Kind == TokenKind.EscapedIdentifier)
		{
			// yeah, escaped identifier is a string token, shame on me
			return new StringToken(_syntaxTree, info.Kind, span, text!, StringLiteralType.None, leading, trailing);
		}
		if (info.Kind == TokenKind.StringLiteral ||
		    info.Kind == TokenKind.CharacterLiteral)
		{
			return new StringToken(_syntaxTree, info.Kind, span, text!, info.StringType, leading, trailing);
		}

		if (info.Kind == TokenKind.LifetimeIdentifier)
		{
			return new StringToken(_syntaxTree, info.Kind, span, text!, StringLiteralType.None, leading, trailing);
		}
		if (info.Kind == TokenKind.NumberLiteral)
		{
			return new NumberToken(_syntaxTree, span,
				NumberParser.Parse(
					info,
					text,
					Location.Create(_syntaxTree, span),
					_diagnostics),
				info.NumericType,
				info.NumericFormat,
				leading,
				trailing);
		}

		return new Token.Default(_syntaxTree, info.Kind, span, leading, trailing);
	}

	private void ReadToken(ref TokenInfo info)
	{
		if (_window.IsAtTheEnd)
		{
			info.Kind = TokenKind.EndOfFile;
			return;
		}

		switch (_window.Current)
		{
			case >= 'a' and <= 'z':
			case >= 'A' and <= 'Z':
				ReadIdentifier(ref info);
				break;
			case '~':
				_window.Advance();
				if (_window.Current == '=')
				{
					info.Kind = TokenKind.TildeAssignment;
					_window.Advance();
				}
				else
				{
					info.Kind = TokenKind.Tilde;
				}

				break;
			case ':':
				_window.Advance();
				if (_window.Current == ':')
				{
					info.Kind = TokenKind.DoubleColon;
					_window.Advance();
				}
				else
				{
					info.Kind = TokenKind.Colon;
				}

				break;
			case '?':
				if (_window.Next == '?')
				{
					_window.Advance(2);
					info.Kind = TokenKind.DoubleQuestionSign;
				}
				else if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.QuestionAssignmentSign;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.QuestionSign;
				}

				break;
			case '-':
				if (_window.Next == '>')
				{
					_window.Advance(2);
					info.Kind = TokenKind.Retusa;
				}
				else if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.MinusAssignment;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Minus;
				}

				break;
			case '+':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.PlusAssignment;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Plus;
				}

				break;
			case '*':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.AsteriskAssignment;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Asterisk;
				}

				break;
			case '/':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.SlashAssignment;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Slash;
				}

				break;
			case '&':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.AmpersandAssignment;
				}
				else if (_window.Next == '&')
				{
					_window.Advance(2);
					info.Kind = TokenKind.DoubleAmpersand;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Ampersand;
				}

				break;
			case '!':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.NotEqual;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.ExclamationSign;
				}

				break;
			case '%':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.PercentAssignment;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Percent;
				}

				break;
			case '|':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.PipeAssignment;
				}
				else if (_window.Next == '|')
				{
					_window.Advance(2);
					info.Kind = TokenKind.DoublePipe;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Pipe;
				}

				break;
			case '^':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.CircumflexAssignment;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Circumflex;
				}

				break;
			case '=':
				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.DoubleEqual;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Equal;
				}

				break;
			case '<':
				if (_window.Next == '<')
				{
					if (_window.Peek(2) == '=') // <<=
					{
						_window.Advance(3);
						info.Kind = TokenKind.LeftArithmeticShiftAssignment;
					}
					else
					{
						_window.Advance(2);
						info.Kind = TokenKind.LeftArithmeticShift;
					}
				}
				else if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.LessOrEquals;
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Less;
				}

				break;
			case '>':
				// Lexer can't distinct >> (right shift) and >> (two generic list terminators) out of context
				// so we do it on Parser 🤣

				if (_window.Next == '=')
				{
					_window.Advance(2);
					info.Kind = TokenKind.GreaterOrEquals;
				}
				else
				{
					_window.Advance(1);
					info.Kind = TokenKind.Greater;
				}

				break;
			case '\"':
				ReadString(ref info);
				break;
			case ';':
				_window.Advance();
				info.Kind = TokenKind.Semicolon;
				break;
			case '{':
				_window.Advance();
				info.Kind = TokenKind.OpenBrace;
				break;
			case '}':
				_window.Advance();
				info.Kind = TokenKind.CloseBrace;
				break;
			case '(':
				_window.Advance();
				info.Kind = TokenKind.OpenParen;
				break;
			case ')':
				_window.Advance();
				info.Kind = TokenKind.CloseParen;
				break;
			case '[':
				_window.Advance();
				info.Kind = TokenKind.OpenBracket;
				break;
			case ']':
				_window.Advance();
				info.Kind = TokenKind.CloseBracket;
				break;
			case ',':
				_window.Advance();
				info.Kind = TokenKind.Comma;
				break;
			case '.':
				if (_window.Next == '.')
				{
					if (_window.Peek(2) == '=')
					{
						_window.Advance(3);
						info.Kind = TokenKind.RangeInclusive;
					}
					else
					{
						_window.Advance(2);
						info.Kind = TokenKind.Range;
					}
				}
				else
				{
					_window.Advance();
					info.Kind = TokenKind.Dot;
				}

				break;
			case '\'':
				ReadCharacterOrLifetime(ref info);
				break;
			case >= '0' and <= '9':
				ReadNumber(ref info);
				break;
			case '`':
				ReadEscapedIdentifier(ref info);
				break;
			default:
				ReadIdentifier(ref info);

				if (_window.Width == 0)
				{
					_window.Advance();
					info.Kind = TokenKind.None;
				}

				break;
		}
	}
}