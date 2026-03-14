using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal sealed class BoundBinaryExpression : BoundExpression
{
	public BoundExpression Left { get; }
	public BinaryOperatorSignature Op { get; }
	public BoundExpression Right { get; }

	public override BoundKind Kind => BoundKind.BinaryExpression;
	public override TypeSymbol Type => Op.ReturnType;

	public BoundBinaryExpression(SyntaxNode syntax, BoundExpression left, BinaryOperatorSignature op, BoundExpression right)
		: base(syntax)
	{
		Left = left;
		Op = op;
		Right = right;
	}

	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;
}