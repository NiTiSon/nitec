using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class QualifiedNameSyntax(SyntaxTree tree, IdentifierNameSyntax left, SyntaxToken dot, NameSyntax right) : NameSyntax(tree)
{
	public IdentifierNameSyntax Left { get; } = left;
	public SyntaxToken Dot { get; } = dot;
	public NameSyntax Right { get; } = right;

	public override SyntaxKind Kind => SyntaxKind.QualifiedName;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return Dot;
		yield return Right;
	}
}