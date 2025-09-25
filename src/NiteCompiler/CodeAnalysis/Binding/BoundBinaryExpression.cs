using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundBinaryExpression : BoundExpression
{
	public BoundBinaryExpression(SyntaxNode syntax, BoundExpression left, BoundBinaryOperator op, BoundExpression right) : base(syntax)
	{
		Left = left;
		Operator = op;
		Right = right;
	}

	public override BoundKind Kind => BoundKind.BinaryExpression;
	public override TypeSymbol Type => Operator.Type;
	public BoundExpression Left { get; }
	public BoundBinaryOperator Operator { get; }
	public BoundExpression Right { get; }
}