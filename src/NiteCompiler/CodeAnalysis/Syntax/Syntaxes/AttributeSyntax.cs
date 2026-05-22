using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class AttributeSyntax : SyntaxNode
{
	public Token Name { get; }
	public Token? OpenParen { get; }
	public SyntaxList<Token>? Arguments { get; }
	public Token? CloseParen { get; }

	public override TextSpan Span
	{
		get
		{
			if (CloseParen != null)
				return TextSpan.FromBounds(Name.Span.Start, CloseParen.Span.End);
			return Name.Span;
		}
	}

	public override NodeKind Kind => NodeKind.Attribute;

	internal AttributeSyntax(SyntaxTree tree, Token name,
		Token? openParen, SyntaxList<Token>? arguments, Token? closeParen) : base(tree)
	{
		Debug.Assert(name.IsKeyword || name.TKind.IsAnyIdentifierOrKeyword);
		Name = name;
		OpenParen = openParen;
		Arguments = arguments;
		CloseParen = closeParen;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitAttribute(this);

	public override void Accept(SyntaxVisitor visitor) =>
		visitor.VisitAttribute(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		if (OpenParen != null) yield return OpenParen;
		if (Arguments != null) yield return Arguments;
		if (CloseParen != null) yield return CloseParen;
	}
}
