using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public abstract class MemberDeclarationSyntax : SyntaxNode
{
	private protected MemberDeclarationSyntax(SyntaxTree syntaxTree, ImmutableArray<SyntaxToken> modifications) : base(syntaxTree)
	{
		Modifications = modifications;
	}

	public ImmutableArray<SyntaxToken> Modifications { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		foreach (var modification in Modifications)
			yield return modification;
	}
}