using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class UnsizedArrayTypeSyntax : TypeSyntax
{
	public Token OpenBracket { get; }
	public TypeSyntax Type { get; }
	public Token CloseBracket { get; }

	public override NodeKind Kind => NodeKind.UnsizedArrayType;
	public override TextSpan Span => TextSpan.FromBounds(OpenBracket.Span, CloseBracket.Span);

	internal UnsizedArrayTypeSyntax(SyntaxTree tree, Token openBracket, TypeSyntax type, Token closeBracket) : base(tree)
	{
		OpenBracket = openBracket;
		Type = type;
		CloseBracket = closeBracket;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUnsizedArrayType(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitUnsizedArrayType(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBracket;
		yield return Type;
		yield return CloseBracket;
	}
}
