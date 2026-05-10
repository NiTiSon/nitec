using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TypeClauseSyntax : SyntaxNode
{
	public Token Token { get; }
	public TypeSyntax Type { get; }

	public override TextSpan Span => TextSpan.FromBounds(Token.Span, Type.Span);
	public override NodeKind Kind => NodeKind.TypeClause;

	internal TypeClauseSyntax(SyntaxTree tree, Token token, TypeSyntax type) : base(tree)
	{
		Token = token;
		Type = type;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeClause(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitTypeClause(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Token;
		yield return Type;
	}
}