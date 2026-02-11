using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FunctionSyntax : ItemSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public SimpleNameSyntax Name { get; }
	public FunctionBodySyntax Body { get; }

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, Body.Span);
	public override NodeKind Kind => NodeKind.Function;

	public FunctionSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers, SimpleNameSyntax name, FunctionBodySyntax body) : base(tree)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		Name = name;
		Body = body;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return Name;
		yield return Body;
	}
}