using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private bool StoreTrivia => DocumentationMode != DocumentationMode.None;

	private void ReadTrivia(bool leading, SyntaxList<Trivia>.Builder trivia)
	{
		bool done = false;

		while (!done)
		{
			_window.Start();

			switch (_window.Current)
			{
				case SlidingWindow.InvalidCharacter:
					done = true;
					break;
				case '#' when _window.Next == '!':
					ReadShebang();
					if (StoreTrivia)
					{
						trivia.Add(new Trivia(_syntaxTree, TokenKind.Shebang, _window.LexemeSpan));
					}
					break;
				case '/':
					switch (_window.Next)
					{
						case '/':
							if (_window.Peek(2) == '/' && DocumentationMode == DocumentationMode.Parse)
							{
								ReadDocsComment();
								if (StoreTrivia) trivia.Add(new Trivia(_syntaxTree, TokenKind.DocsComment, _window.LexemeSpan));
							}
							else
							{
								ReadSingleLineComment();
								if (StoreTrivia) trivia.Add(new Trivia(_syntaxTree, TokenKind.SingleLineComment, _window.LexemeSpan));
							}
							break;
						case '*':
							ReadMultiLineComment();
							if (StoreTrivia) trivia.Add(new Trivia(_syntaxTree, TokenKind.MultiLineComment, _window.LexemeSpan));
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
					if (StoreTrivia) trivia.Add(new Trivia(_syntaxTree, TokenKind.LineBreak, _window.LexemeSpan));
					break;
				case ' ':
				case '\t':
					ReadWhiteSpace();
					if (StoreTrivia) trivia.Add(new Trivia(_syntaxTree, TokenKind.Whitespace, _window.LexemeSpan));
					break;
				default:
					if (char.IsWhiteSpace(_window.Current))
					{
						ReadWhiteSpace();
						if (StoreTrivia) trivia.Add(new Trivia(_syntaxTree, TokenKind.Whitespace, _window.LexemeSpan));
					}
					else
						done = true;
					break;
			}
		}
	}

	private void ReadShebang()
	{
		ReadSingleLineComment();
	}

	private void ReadLineBreak()
	{
		_window.AdvancePastNewLine();
	}

	private void ReadWhiteSpace()
	{
		bool done = false;

		while (!done)
		{
			switch (_window.Current)
			{
				case SlidingWindow.InvalidCharacter:
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
	}

	private void ReadDocsComment()
	{
		ReadSingleLineComment();
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
	}

	private void ReadMultiLineComment()
	{
		_window.Advance(2);
		bool done = false;

		while (!done)
		{
			switch (_window.Current)
			{
				case SlidingWindow.InvalidCharacter:
					TextSpan span = _window.LexemeSpan;
					_diagnostics.ReportNotTerminatedMultiLineComment(span.Contextualize(_syntaxTree));
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
	}
}