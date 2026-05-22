using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ConstructorDeclarationSyntax : BaseConstructorDeclarationSyntax
{
	public override ParameterListSyntax ParameterList { get; }

	public override NodeKind Kind => NodeKind.ConstructorDeclaration;

	internal ConstructorDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers,
		Token newKeyword, ParameterListSyntax parameterList, FunctionBodySyntax body,
		SyntaxList<AttributeListSyntax>? attributes = null)
		: base(tree, accessibilityToken, modifiers, newKeyword, body, attributes)
	{
		ParameterList = parameterList;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return NewKeyword;
		yield return ParameterList;
		yield return Body;
	}
}
