using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class NumericLiteralExpression : ExpressionSyntax
{
	public NumericLiteralExpression(SyntaxTree tree, SyntaxToken numericLiteral) : base(tree)
	{
		Debug.Ensure(numericLiteral, SyntaxKind.NumericLiteralToken);
		Literal = numericLiteral;
	}

	public override SyntaxKind Kind => SyntaxKind.NumericLiteralExpression;
	public SyntaxToken Literal { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Literal;
	}
}