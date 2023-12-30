using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode, ICompilationUnitSyntax
{
	public CompilationUnitSyntax(SyntaxTree tree, ImmutableArray<MemberDeclarationSyntax> members, SyntaxNode endOfFileToken) : base(tree)
	{
		Members = members;
		EndOfFileToken = endOfFileToken;
	}

	public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

	public ImmutableArray<MemberDeclarationSyntax> Members { get; }
	public SyntaxNode EndOfFileToken { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		foreach (MemberDeclarationSyntax member in Members)
			yield return member;

		yield return EndOfFileToken;
	}
}
