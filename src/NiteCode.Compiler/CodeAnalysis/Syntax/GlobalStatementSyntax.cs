using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class GlobalStatementSyntax : MemberDeclarationSyntax
{
	public GlobalStatementSyntax(SyntaxTree tree, SyntaxNode statement) : base(tree)
	{
		Statement = statement;
	}

	public SyntaxNode Statement { get; }

	public override SyntaxKind Kind => SyntaxKind.GlobalStatement;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Statement;
	}
}