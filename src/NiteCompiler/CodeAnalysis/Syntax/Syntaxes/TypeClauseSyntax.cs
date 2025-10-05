using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TypeClauseSyntax : SyntaxNode
{
	public TypeClauseSyntax(Token colon, TypeSyntax type)
	{
		Colon = colon;
		Type = type;
	}

	public Token Colon { get; }
	public TypeSyntax Type { get; }
	public override TextSpan Span => TextSpan.FromBounds(Colon.Span, Type.Span);
	public override SyntaxKind Kind => SyntaxKind.TypeClause;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Colon;
		yield return Type;
	}
}