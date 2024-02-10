using Microsoft.Extensions.Primitives;
using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class SyntaxToken : SyntaxNode
{
	public SyntaxToken(SyntaxTree tree, SyntaxKind kind, uint position, StringSegment text, ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia) : base(tree)
	{
		Kind = kind;
		Position = position;
		Text = text;
		LeadingTrivia = leadingTrivia;
		TrailingTrivia = trailingTrivia;
		IsMissing = false;
	}

	/// <summary>
	/// Interprets this token as an identifier.
	/// </summary>
	public SyntaxToken ToIdentifier()
	{
		return new SyntaxToken(SyntaxTree, SyntaxKind.IdentifierToken, Position, Text, LeadingTrivia, TrailingTrivia);
	}

	public override SyntaxKind Kind { get; }
	public uint Position { get; }
	public StringSegment Text { get; }
	public override TextSpan Span => new(Position, (uint)Text.Length);
	public override TextSpan FullSpan
	{
		get
		{
			uint start = LeadingTrivia.Length == 0
							? Span.Position
							: LeadingTrivia.First().Span.Position;
			uint end = TrailingTrivia.Length == 0
							? Span.EndPosition
							: TrailingTrivia.Last().Span.EndPosition;
			return TextSpan.FromBounds(start, end);
		}
	}

	public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }
	public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return Array.Empty<SyntaxNode>();
	}

	/// <summary>
	/// A token is missing if it was inserted by the parser and doesn't appear in source.
	/// </summary>
	public bool IsMissing { get; }
}