using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
{
	public ParenthesizedExpressionSyntax(SyntaxTree tree, SyntaxToken openParen, ExpressionSyntax expression, SyntaxToken closeParen) : base(tree)
	{
		Debug.Ensure(openParen, SyntaxKind.OpenParenToken);
		Debug.Ensure(closeParen, SyntaxKind.CloseParenToken);

		OpenParen = openParen;
		Expression = expression;
		CloseParen = closeParen;
	}
	public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

	public SyntaxToken OpenParen { get; }
	public ExpressionSyntax Expression { get; }
	public SyntaxToken CloseParen { get; }


	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenParen;
		yield return Expression;
		yield return CloseParen;
	}
}