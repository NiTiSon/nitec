using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class WrongExpressionSyntax : ExpressionSyntax
{
	public override SyntaxKind Kind => SyntaxKind.WrongExpression;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield break;
	}
}