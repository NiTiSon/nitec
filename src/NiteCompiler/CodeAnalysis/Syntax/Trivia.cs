using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class Trivia : SyntaxNode
{
	public override TextSpan Span { get; }
	public override NodeKind Kind => NodeKind.Trivia;
	public TokenKind TKind { get; }

	internal Trivia(SyntaxTree tree, TokenKind kind, TextSpan span) : base(tree)
	{
		Span = span;
		TKind = kind;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTrivia(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitTrivia(this);


	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return [];
	}
}