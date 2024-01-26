using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
	public BinaryExpressionSyntax(SyntaxTree tree, SyntaxKind kind, ExpressionSyntax left, SyntaxToken operationToken, ExpressionSyntax right) : base(tree)
	{
		Left = left;
		OperationToken = operationToken;
		Right = right;
		Kind = kind;
	}

	public ExpressionSyntax Left { get; }
	public SyntaxToken OperationToken { get; }
	public ExpressionSyntax Right { get; }

	public override SyntaxKind Kind { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return OperationToken;
		yield return Right;
	}
}