using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;

namespace Nlr.Compiler.CodeAnalysis.Syntax;

public readonly struct Token : ISyntaxNode
{
	public readonly SyntaxKind Kind;
	public readonly string Value;
	public readonly ImmutableArray<Trivia> LeadingTrivia;
	public readonly ImmutableArray<Trivia> TrailingTrivia;

	public Token(SyntaxKind kind, string value, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia)
	{
		Kind = kind;
		Value = value;
		LeadingTrivia = leadingTrivia;
		TrailingTrivia = trailingTrivia;
	}

	public override string ToString()
	{
		return $"{Kind}: {(Value is null ? string.Empty : $"'{Value}'")} lead: {LeadingTrivia.Length} trail: {TrailingTrivia.Length}";
	}
}