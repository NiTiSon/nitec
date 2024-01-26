using Microsoft.Extensions.Primitives;
using NiteCode.Compiler.CodeAnalysis.Text;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class SyntaxTrivia
{
	internal SyntaxTrivia(SyntaxTree syntaxTree, SyntaxKind kind, uint position, StringSegment text)
	{
		SyntaxTree = syntaxTree;
		Kind = kind;
		Position = position;
		Text = text;
	}

	public SyntaxTree SyntaxTree { get; }
	public SyntaxKind Kind { get; }
	public StringSegment Text { get; }
	public uint Position { get; }

	public TextSpan Span => new(Position, (uint)Text.Length);
}
