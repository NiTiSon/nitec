using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FieldDeclarationSyntax : MemberSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public SimpleNameSyntax Name { get; }
	public TypeClauseSyntax TypeClause { get; }
	public Token SemicolonToken { get; }

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, SemicolonToken.Span);
	public override NodeKind Kind => NodeKind.FieldDeclaration;

	internal FieldDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers, SimpleNameSyntax name,
		TypeClauseSyntax typeClause, Token semicolonToken) : base(tree)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		Name = name;
		TypeClause = typeClause;
		SemicolonToken = semicolonToken;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFieldDeclaration(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitFieldDeclaration(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return Name;
		yield return TypeClause;
		yield return SemicolonToken;
	}
}
