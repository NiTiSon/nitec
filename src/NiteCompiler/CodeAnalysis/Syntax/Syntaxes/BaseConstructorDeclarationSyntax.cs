using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class BaseConstructorDeclarationSyntax : MemberSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public Token NewKeyword { get; }
	public abstract ParameterListSyntax ParameterList { get; }
	public FunctionBodySyntax Body { get; }

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, Body.Span);

	private protected BaseConstructorDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers,
		Token newKeyword, FunctionBodySyntax body, SyntaxList<AttributeListSyntax>? attributes = null) : base(tree, attributes)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		NewKeyword = newKeyword;
		Body = body;
	}

	public sealed override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstructorDeclaration(this);
	public sealed override void Accept(SyntaxVisitor visitor) => visitor.VisitConstructorDeclaration(this);
}
