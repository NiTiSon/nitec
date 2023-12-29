using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class IdentifierNameSyntax(SyntaxTree tree, SyntaxToken identifier) : NameSyntax(tree)
{
	public SyntaxToken Identifier { get; } = identifier;

	public override SyntaxKind Kind => SyntaxKind.IdentifierName;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
	}
}