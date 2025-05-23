using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class LoopStatementSyntax : StatementSyntax, IScopeDefyingStatement
{
	public Token LoopKeyword { get; }
	
	public StatementSyntax Body { get; }

	public LoopStatementSyntax(Token loopKeyword, StatementSyntax body)
	{
		LoopKeyword = loopKeyword;
		Body = body;
	}
	
	public override SyntaxKind Kind => SyntaxKind.LoopStatement;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return LoopKeyword;
		yield return Body;
	}
}