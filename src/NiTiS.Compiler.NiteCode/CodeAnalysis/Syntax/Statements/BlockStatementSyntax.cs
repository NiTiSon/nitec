using System.Collections.Generic;
using System.Collections.Immutable;
using NiTiS.Compiler.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax.Statements;

public sealed class BlockStatementSyntax : StatementSyntax
{
	public readonly NiteToken OpenParenToken;
	public readonly ImmutableArray<StatementSyntax> Statements;
	public readonly NiteToken CloseParenToken;

	public BlockStatementSyntax(NiteToken openParenToken, ImmutableArray<StatementSyntax> statements, NiteToken closeParenToken)
	{
		OpenParenToken = openParenToken;
		Statements = statements;
		CloseParenToken = closeParenToken;
	}

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return OpenParenToken;
		foreach (var statement in Statements)
		{
			yield return statement;
		}
		yield return CloseParenToken;
	}
}