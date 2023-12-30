using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class BlockStatementSyntax(SyntaxTree tree, SyntaxToken openBrace, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBrace) : StatementSyntax(tree)
{
	public override SyntaxKind Kind => SyntaxKind.BlockStatement;

	public SyntaxToken Open { get; } = openBrace;
	public ImmutableArray<StatementSyntax> Statements { get; } = statements;
	public SyntaxToken Close { get; } = closeBrace;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Open;

		foreach (var statement in Statements)
			yield return statement;
		
		yield return Close;
	}
}