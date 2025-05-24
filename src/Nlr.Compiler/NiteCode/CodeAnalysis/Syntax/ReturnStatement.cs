using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ReturnStatement : StatementSyntax
{
	public readonly Token ReturnKeyword;

	public readonly ExpressionSyntax? Expression;

	public ReturnStatement(Token returnKeyword, [Optional] ExpressionSyntax expression)
	{
		ReturnKeyword = returnKeyword;
		Expression = expression;
	}

	public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return ReturnKeyword;
		if (Expression != null)
		{
			yield return Expression;
		}
	}
}