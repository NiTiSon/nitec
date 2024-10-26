using Microsoft.Extensions.Primitives;
using Nlr.Compiler.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public readonly struct SyntaxToken
{
	private readonly SyntaxKind kind;
	private readonly StringSegment text;
	private readonly TextSpan span;
	private readonly ImmutableArray<SyntaxTrivia> leadingTrivia;
	private readonly ImmutableArray<SyntaxTrivia> trailingTrivia;

	public SyntaxToken(SyntaxKind kind, StringSegment text, TextSpan span, ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia)
	{
		this.kind = kind;
		this.text = text;
		this.span = span;
		this.leadingTrivia = leadingTrivia;
		this.trailingTrivia = trailingTrivia;
	}

	public TextSpan Span => span;

	public TextSpan FullSpan
	{
		get
		{
			uint start = leadingTrivia.Length == 0
				? Span.Begin
				: leadingTrivia.First().Span.Begin;

			uint end = trailingTrivia.Length == 0
				? Span.End
				: trailingTrivia.Last().Span.End;

			return TextSpan.FromBounds(start, end);
		}
	}

	public SyntaxKind Kind => kind;

	public override string ToString()
	{
		return $"{{{Kind}, {span}{(text.Length > 0 ? ", '" + text.Value + '\'' : string.Empty)}}}";
	}
}