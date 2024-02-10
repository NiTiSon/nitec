using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class BlockStatementSyntax : SyntaxNode
{
	public BlockStatementSyntax(SyntaxTree tree, SyntaxToken openBrace, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBrace) : base(tree)
	{
		Debug.Ensure(openBrace, SyntaxKind.OpenBraceToken);
		Debug.Ensure(closeBrace, SyntaxKind.CloseBraceToken);
		OpenBrace = openBrace;
		Statements = statements;
		CloseBrace = closeBrace;
	}

	public SyntaxToken OpenBrace { get; }
	public ImmutableArray<StatementSyntax> Statements { get; }
	public SyntaxToken CloseBrace { get; }

	public override SyntaxKind Kind => SyntaxKind.BlockStatement;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBrace;
		
		foreach (StatementSyntax statement in Statements)
			yield return statement;

		yield return CloseBrace;
	}
}