using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ArrayTypeSyntax : TypeSyntax
{
	public Token OpenBracket { get; }
	public TypeSyntax Type { get; }
	public Token Comma { get; }
	public ExpressionSyntax Size { get; }
	public Token CloseBracket { get; }

	public override NodeKind Kind => NodeKind.ArrayType;
	public override TextSpan Span => TextSpan.FromBounds(OpenBracket.Span, CloseBracket.Span);

	internal ArrayTypeSyntax(SyntaxTree tree, Token openBracket, TypeSyntax type, Token comma, ExpressionSyntax size, Token closeBracket) : base(tree)
	{
		OpenBracket = openBracket;
		Type = type;
		Comma = comma;
		Size = size;
		CloseBracket = closeBracket;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrayType(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitArrayType(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBracket;
		yield return Type;
		yield return Comma;
		yield return Size;
		yield return CloseBracket;
	}
}
