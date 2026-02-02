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


	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return [];
	}
}