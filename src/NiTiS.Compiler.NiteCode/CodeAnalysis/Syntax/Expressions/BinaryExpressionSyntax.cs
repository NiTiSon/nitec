using System.Collections.Generic;
using NiTiS.Compiler.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax.Expressions;

public class BinaryExpressionSyntax : ExpressionSyntax
{
	public readonly ExpressionSyntax Left;
	public readonly NiteToken Operator;
	public readonly ExpressionSyntax Right;

	public BinaryExpressionSyntax(ExpressionSyntax left, NiteToken @operator, ExpressionSyntax right)
	{
		Left = left;
		Operator = @operator;
		Right = right;
	}

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		throw new System.NotImplementedException();
	}
}