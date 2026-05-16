using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class NamedConstructorDeclarationSyntax : BaseConstructorDeclarationSyntax
{
	public SimpleNameSyntax Name { get; }
	public override ParameterListSyntax ParameterList { get; }

	public override NodeKind Kind => NodeKind.NamedConstructorDeclaration;

	internal NamedConstructorDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers,
		Token newKeyword, SimpleNameSyntax name, ParameterListSyntax parameterList,
		FunctionBodySyntax body) : base(tree, accessibilityToken, modifiers, newKeyword, body)
	{
		Name = name;
		ParameterList = parameterList;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return NewKeyword;
		yield return Name;
		yield return ParameterList;
		yield return Body;
	}
}
