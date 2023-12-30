using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class PredefinedTypeSyntax(SyntaxTree tree, SyntaxToken typeKeyword) : TypeSyntax(tree)
{
	public override SyntaxKind Kind => SyntaxKind.PredefinedType;

	public SyntaxToken Keyword { get; } = typeKeyword;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Keyword;
	}
}
