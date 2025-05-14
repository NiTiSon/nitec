using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class LiteralExpression : ExpressionSyntax
{
	public Token Literal { get; }

	public LiteralExpression(Token token)
	{
		Literal = token;
	}
	
	public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return Literal;
	}
}