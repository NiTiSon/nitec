using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class ParameterSyntax(SyntaxTree tree, TypeSyntax type, IdentifierNameSyntax name) : SyntaxNode(tree)
{
	public override SyntaxKind Kind => SyntaxKind.Parameter;

	public TypeSyntax Type { get; } = type;
	public IdentifierNameSyntax Name { get; } = name;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Type;
		yield return Name;
	}
}