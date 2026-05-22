using System;
using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TypeDeclarationSyntax : MemberSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public Token TypeKeyword { get; }
	public NameSyntax Name { get; }
	public GenericParameterListSyntax? GenericParameterList { get; }
	public TypeBodySyntax Body { get; }
	public SyntaxList<MemberSyntax>? Members => Body is MembersTypeBodySyntax members ? members.Members : null;

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, Body.Span);
	public override NodeKind Kind => NodeKind.TypeDeclaration;

	internal TypeDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers,
		Token typeKeyword, NameSyntax name, GenericParameterListSyntax? genericParameterList, TypeBodySyntax body,
		SyntaxList<AttributeListSyntax>? attributes = null) : base(tree, attributes)
	{
		Debug.Assert(name is SimpleNameSyntax or InlineNameSyntax);

		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		TypeKeyword = typeKeyword;
		Name = name;
		GenericParameterList = genericParameterList;
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
		if (GenericParameterList != null) yield return GenericParameterList;
		yield return Body;
	}
}