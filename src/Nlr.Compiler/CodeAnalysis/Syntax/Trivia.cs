using Microsoft.Extensions.Primitives;

namespace Nlr.Compiler.CodeAnalysis.Syntax;

public readonly struct Trivia
{
	public readonly SyntaxKind Kind;
	public readonly string Text;
	public readonly int Position;

	public Trivia(SyntaxKind kind, int position, string text)
	{
		Kind = kind;
		Text = text;
		Position = position;
	}
}