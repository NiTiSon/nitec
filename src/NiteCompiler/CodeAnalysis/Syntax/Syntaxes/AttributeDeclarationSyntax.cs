using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class AttributeDeclarationSyntax : MemberSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public Token AttributeKeyword { get; }
	public SimpleNameSyntax Name { get; }
	public ParameterListSyntax? ParameterList { get; }
	public Token? ForKeyword { get; }
	public SyntaxList<Token>? Targets { get; }
	public Token SemicolonToken { get; }

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, SemicolonToken.Span);
	public override NodeKind Kind => NodeKind.AttributeDeclaration;

	internal AttributeDeclarationSyntax(SyntaxTree tree,
		Token accessibilityToken, SyntaxList<Token> modifiers,
		Token attributeKeyword, SimpleNameSyntax name,
		ParameterListSyntax? parameterList,
		Token? forKeyword, SyntaxList<Token>? targets,
		Token semicolonToken) : base(tree)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		AttributeKeyword = attributeKeyword;
		Name = name;
		ParameterList = parameterList;
		ForKeyword = forKeyword;
		Targets = targets;
		SemicolonToken = semicolonToken;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitAttributeDeclaration(this);

	public override void Accept(SyntaxVisitor visitor) =>
		visitor.VisitAttributeDeclaration(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return AttributeKeyword;
		yield return Name;
		if (ParameterList != null) yield return ParameterList;
		if (ForKeyword != null) yield return ForKeyword;
		if (Targets != null) yield return Targets;
		yield return SemicolonToken;
	}
}
