using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode, ICompilationUnitSyntax, IAccessZone
{
	public CompilationUnitSyntax(SyntaxTree tree, ImmutableArray<MemberSyntax> members, ImmutableArray<UsingSyntax> usings, SyntaxNode endOfFileToken) : base(tree)
	{
		Members = members;
		Usings = usings;
		EndOfFileToken = endOfFileToken;
		Usings = usings;
	}

	public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

	public ImmutableArray<MemberSyntax> Members { get; }
	public ImmutableArray<UsingSyntax> Usings { get; }
	public SyntaxNode EndOfFileToken { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		foreach (UsingSyntax @using in Usings)
			yield return @using;
		
		foreach (MemberSyntax member in Members)
			yield return member;

		yield return EndOfFileToken;
	}
}
