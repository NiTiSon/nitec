using System.Collections.Generic;
using System.Collections.Immutable;
using NiTiS.Compiler.CodeAnalysis.Text;

namespace NiTiS.Compiler.CodeAnalysis.Syntax;

public abstract class Token : ISyntaxNode
{
	public readonly uint RawKind;
	public readonly uint RawContextualKind;
	public readonly TextSpan Span;
	public readonly ImmutableArray<Trivia> LeadingTrivia;
	public readonly ImmutableArray<Trivia> TrailingTrivia;

	protected Token(uint rawKind, TextSpan span, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia)
	{
		RawKind = rawKind;
		RawContextualKind = 0;
		LeadingTrivia = leadingTrivia;
		TrailingTrivia = trailingTrivia;
		Span = span;
	}

	public Token(uint rawKind, uint rawContextualKind, TextSpan span, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia)
	{
		RawKind = rawKind;
		RawContextualKind = rawContextualKind;
		LeadingTrivia = leadingTrivia;
		TrailingTrivia = trailingTrivia;
		Span = span;
	}

	public IEnumerable<ISyntaxNode> GetChildren() => [];
}