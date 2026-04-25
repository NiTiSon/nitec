using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TypeDeclarationSyntax : MemberSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public Token TypeKeyword { get; }
	public SimpleNameSyntax Name { get; }
	public TypeBodySyntax Body { get; }
	public SyntaxList<MemberSyntax>? Members => Body is MembersTypeBodySyntax members ? members.Members : null;

	public Token ClosingToken => Body.ClosingToken;

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, Body.Span);
	public override NodeKind Kind => NodeKind.TypeDeclaration;

	internal TypeDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers, Token typeKeyword, SimpleNameSyntax name, TypeBodySyntax body) : base(tree)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		TypeKeyword = typeKeyword;
		Name = name;
		Body = body;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeDeclaration(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitTypeDeclaration(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return TypeKeyword;
		yield return Name;
		yield return Body;
	}
}