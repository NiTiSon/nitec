using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Text;

namespace Nlr.Compiler.CodeAnalysis.Syntax;

public readonly struct Token : ISyntaxNode
{
	public readonly SyntaxKind Kind;
	public readonly TextSpan Span;
	public readonly string Value;
	public readonly ImmutableArray<Trivia> LeadingTrivia;
	public readonly ImmutableArray<Trivia> TrailingTrivia;

	public Token(SyntaxKind kind, TextSpan span, string value, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia)
	{
		Kind = kind;
		Value = value;
		LeadingTrivia = leadingTrivia;
		TrailingTrivia = trailingTrivia;
		Span = span;
	}

	public override string ToString()
	{
		return $"{Kind}: {(Value is null ? string.Empty : $"'{Value}'")} lead: {LeadingTrivia.Length} trail: {TrailingTrivia.Length}";
	}

	SyntaxKind ISyntaxNode.Kind => Kind;

	public IEnumerable<ISyntaxNode> GetChildren() => [];
}