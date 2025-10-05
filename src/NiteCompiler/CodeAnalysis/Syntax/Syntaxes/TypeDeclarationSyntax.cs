using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TypeDeclarationSyntax : MemberSyntax
{
	public Token TypeKeyword { get; }
	public SimpleNameSyntax Name { get; }
	public Token OpenBraceToken { get; }
	public ImmutableArray<MemberSyntax> Members { get; }
	public Token CloseBraceToken { get; }
	public TypeDeclarationSyntax(Token accessibilityToken, ImmutableArray<Token> modifiers, Token typeKeyword,
		SimpleNameSyntax name,
		Token openBrace, ImmutableArray<MemberSyntax> members, Token closeBrace) : base(accessibilityToken, modifiers)
	{
		TypeKeyword = typeKeyword;
		Name = name;
		OpenBraceToken = openBrace;
		Members = members;
		CloseBraceToken = closeBrace;
	}

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, CloseBraceToken.Span);
	public override SyntaxKind Kind => SyntaxKind.TypeDeclaration;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		foreach (var modifier in Modifiers)
		{
			yield return modifier;
		}
		yield return Name;
		yield return OpenBraceToken;
		foreach (var member in Members)
		{
			yield return member;
		}
		yield return CloseBraceToken;
	}
}