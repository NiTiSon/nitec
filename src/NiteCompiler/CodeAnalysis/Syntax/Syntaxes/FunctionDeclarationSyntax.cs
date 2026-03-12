using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FunctionDeclarationSyntax : MemberSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public SimpleNameSyntax Name { get; }
	public TypeClause? TypeClause { get; }
	public FunctionBodySyntax Body { get; }

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, Body.Span);
	public override NodeKind Kind => NodeKind.FunctionDeclaration;

	public FunctionDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers, SimpleNameSyntax name, TypeClause? typeClause, FunctionBodySyntax body) : base(tree)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		Name = name;
		TypeClause = typeClause;
		Body = body;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionDeclaration(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitFunctionDeclaration(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return Name;
		if (TypeClause != null) yield return TypeClause;
		yield return Body;
	}
}