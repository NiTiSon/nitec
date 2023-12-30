using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class FunctionDeclarationSyntax(SyntaxTree tree, ImmutableArray<SyntaxToken> modifications, TypeSyntax returnType, IdentifierNameSyntax name, ParameterListSyntax parameters, BlockStatementSyntax body) : MemberDeclarationSyntax(tree, modifications)
{
	public override SyntaxKind Kind => SyntaxKind.Function;

	public TypeSyntax ReturnType { get; } = returnType;
	public IdentifierNameSyntax Name { get; } = name;
	public ParameterListSyntax Parameters { get; } = parameters;
	public BlockStatementSyntax Body { get; } = body;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		foreach (SyntaxNode node in base.GetChildren())
			yield return node;

		yield return ReturnType;
		yield return Name;
		yield return Parameters;
		yield return Body;
	}
}
