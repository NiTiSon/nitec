using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class IdentifierNameSyntax : NameSyntax
{
	public IdentifierNameSyntax(SyntaxTree tree, SyntaxToken identifier) : base(tree)
	{
		Debug.Ensure(identifier, SyntaxKind.IdentifierToken);
		Identifier = identifier;
	}

	public SyntaxToken Identifier { get; }

	public override SyntaxKind Kind => SyntaxKind.IdentifierName;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
	}
}
