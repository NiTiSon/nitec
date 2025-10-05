using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FieldDeclarationSyntax : MemberSyntax
{
	public SimpleNameSyntax Name { get; }
	public TypeClauseSyntax? TypeClause { get; }
	public ExpressionSyntax? Initializer { get; }

	public FieldDeclarationSyntax(Token accessibilityToken, ImmutableArray<Token> modifiers, SimpleNameSyntax name, TypeClauseSyntax? typeClause,
		ExpressionSyntax? initializer) : base(accessibilityToken, modifiers)
	{
		Name = name;
		TypeClause = typeClause;
		Initializer = initializer;
	}

	public override TextSpan Span
	{
		get
		{
			SyntaxNode last = Initializer ?? (SyntaxNode?)TypeClause ?? (Modifiers.Length > 0 ? Modifiers[^1] : AccessibilityToken);
			return TextSpan.FromBounds(AccessibilityToken.Span, last.Span);
		}
	}
	public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		foreach (var modifier in Modifiers)
		{
			yield return modifier;
		}
		if (TypeClause != null) yield return TypeClause;
		if (Initializer != null) yield return Initializer;
	}
}