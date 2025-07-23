using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private void ReadTrivia(bool leading)
	{
		TriviaBuilder.Clear();

		bool done = false;

		while (!done)
		{
			Window.Start();
			_kind = SyntaxKind.Invalid;

			switch (Window.Current)
			{
				case SlidingWindow.InvalidCharacter:
					done = true;
					break;
				case '/':
					switch (Window.Next)
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
					if (char.IsWhiteSpace(Window.Current))
						ReadWhiteSpace();
					else
						done = true;
					break;
			}

			if (Window.Width <= 0) continue;

			Trivia trivia = Trivia.Create(_kind, Window.LexemeSpan);
			TriviaBuilder.Add(trivia);
		}
	}

	private void ReadLineBreak()
	{
		_kind = SyntaxKind.LineBreakTrivia;
		Window.AdvancePastNewLine();
	}

	private void ReadWhiteSpace()
	{
		bool done = false;

		while (!done)
		{
			switch (Window.Current)
			{
				case SlidingWindow.InvalidCharacter:
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					if (!char.IsWhiteSpace(Window.Current))
						done = true;
					else
						Window.Advance();
					break;
			}
		}

		_kind = SyntaxKind.WhitespaceTrivia;
	}

	private void ReadSingleLineComment()
	{
		Window.Advance(2);
		bool done = false;

		while (!done)
		{
			switch (Window.Current)
			{
				case '\0':
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					Window.Advance();
					break;
			}
		}

		_kind = SyntaxKind.SingleLineCommentTrivia;
	}

	private void ReadMultiLineComment()
	{
		Window.Advance(2);
		bool done = false;

		while (!done)
		{
			switch (Window.Current)
			{
				case SlidingWindow.InvalidCharacter:
					TextSpan span = new(Window.Position, 2);
					//TODO: Diagnostics.ReportNotTerminatedMultiLineComment(span);
					done = true;
					break;
				case '*':
					if (Window.Next == '/')
					{
						Window.Advance();
						done = true;
					}
					Window.Advance();
					break;
				default:
					Window.Advance();
					break;
			}
		}

		_kind = SyntaxKind.MultiLineCommentTrivia;
	}
}