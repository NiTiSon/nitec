using NiTiS.Compiler.CodeAnalysis.Text;

namespace NiTiS.Compiler.CodeAnalysis.Syntax;

public readonly struct Trivia
{
	public readonly uint RawKind;
	public readonly TextSpan Span;

	public Trivia(uint rawKind, TextSpan span)
	{
		RawKind = rawKind;
		Span = span;
	}
}