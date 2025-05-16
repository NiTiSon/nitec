using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class ExpressionStatementSyntax : StatementSyntax
{
	public readonly ExpressionSyntax Expression;

	public ExpressionStatementSyntax(ExpressionSyntax expression)
	{
		Expression = expression;
	}

	public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return Expression;
	}
}