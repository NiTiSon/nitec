using Microsoft.Extensions.Primitives;
using Nlr.Compiler.Text;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public readonly struct SyntaxTrivia
{
	private readonly SyntaxKind kind;
	private readonly TextSpan span;
	private readonly StringSegment text;

	public SyntaxTrivia(SyntaxKind kind, TextSpan span, StringSegment text)
	{
		this.kind = kind;
		this.span = span;
		this.text = text;
	}

	public TextSpan Span => span;

	public StringSegment Text => text;
}