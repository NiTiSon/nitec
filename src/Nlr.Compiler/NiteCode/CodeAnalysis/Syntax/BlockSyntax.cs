using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class BlockSyntax : StatementSyntax
{
	public readonly Token OpenBraceToken;
	public readonly Token CloseBraceToken;
	public readonly ImmutableArray<StatementSyntax> Statements;

	public BlockSyntax(Token openBraceToken, ImmutableArray<StatementSyntax> statements, Token closeBraceToken)
	{
		OpenBraceToken = openBraceToken;
		Statements = statements;
		CloseBraceToken = closeBraceToken;
	}


	public override SyntaxKind Kind => SyntaxKind.BlockStatement;

	public override bool IsRequireSemicolon => false;

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return OpenBraceToken;
		foreach (StatementSyntax statement in Statements)
		{
			yield return statement;
		}
		yield return CloseBraceToken;
	}
}