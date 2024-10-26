using Nlr.Compiler.Text;

namespace Nlr.Compiler.CodeAnalysis;

public abstract class Lexer
{
	protected readonly TextWindow window;

	public Lexer(SourceText source)
	{
		window = new TextWindow(source);
	}
}