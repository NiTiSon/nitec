using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private void ReadTrivia(bool leading)
	{
		SyntaxKind kind;
		TriviaBuilder.Clear();

		bool done = false;

		while (!done)
		{
			Window.Start();
			kind = SyntaxKind.Invalid;

			switch (Window.Current)
			{
				case SlidingWindow.InvalidCharacter:
					done = true;
					break;
				case '/':
					switch (Window.Next)
					{
						case '/':
							ReadSingleLineComment(out kind);
							break;
						case '*':
							ReadMultiLineComment(out kind);
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
					ReadLineBreak(out kind);
					break;
				case ' ':
				case '\t':
					ReadWhiteSpace(out kind);
					break;
				default:
					if (char.IsWhiteSpace(Window.Current))
						ReadWhiteSpace(out kind);
					else
						done = true;
					break;
			}

			if (Window.Width <= 0) continue;

			Trivia trivia = Trivia.Create(kind, Window.LexemeSpan);
			TriviaBuilder.Add(trivia);
		}
	}

	private void ReadLineBreak(out SyntaxKind kind)
	{
		kind = SyntaxKind.LineBreakTrivia;
		Window.AdvancePastNewLine();
	}

	private void ReadWhiteSpace(out SyntaxKind kind)
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

		kind = SyntaxKind.WhitespaceTrivia;
	}

	private void ReadSingleLineComment(out SyntaxKind kind)
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

		kind = SyntaxKind.SingleLineCommentTrivia;
	}

	private void ReadMultiLineComment(out SyntaxKind kind)
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

		kind = SyntaxKind.MultiLineCommentTrivia;
	}
}