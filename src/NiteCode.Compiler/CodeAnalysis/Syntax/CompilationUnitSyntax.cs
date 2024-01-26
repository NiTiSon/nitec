using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
	public CompilationUnitSyntax(SyntaxTree tree, SyntaxNode endOfFileToken) : base(tree)
	{
		EndOfFileToken = endOfFileToken;
	}
	public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

	public SyntaxNode EndOfFileToken { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		//foreach (MemberDeclarationSyntax member in Members)
		//	yield return member;

		yield return EndOfFileToken;
	}
}
