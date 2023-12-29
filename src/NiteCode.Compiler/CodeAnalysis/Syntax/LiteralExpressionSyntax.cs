using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class LiteralExpressionSyntax(SyntaxTree tree, SyntaxToken token) : ExpressionSyntax(tree)
{
	public override SyntaxKind Kind => SyntaxKind.NumberExpression;
	public SyntaxToken NumberToken { get; } = token;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return NumberToken;
	}
}