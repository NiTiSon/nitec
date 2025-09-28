using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private void ReadTrivia(bool leading)
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
		}
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
					TextSpan span = new(_window.Position, 2);
					_diagnostics.ReportNotTerminatedMultiLineComment(span.Contextualize(_source));
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