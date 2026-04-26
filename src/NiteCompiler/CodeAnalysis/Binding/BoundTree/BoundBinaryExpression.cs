using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundBinaryExpression : BoundExpression
{
	public BoundExpression Left { get; }
	public BinaryOperatorSignature Op { get; }
	public BoundExpression Right { get; }

	public override BoundKind Kind => BoundKind.BinaryExpression;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;

	public override Pureness Pureness
	{
		get
		{
			Pureness pureness = Pureness.Pure;

			pureness += Left.Pureness;
			if (Op.CorrespondingFunction != null)
			{
				pureness += Op.CorrespondingFunction.Pureness;
			}
			pureness += Right.Pureness;

			return pureness;
		}
	}
	public override TypeSymbol Type => Op.ReturnType;

	public BoundBinaryExpression(SyntaxNode syntax, BoundExpression left, BinaryOperatorSignature op, BoundExpression right)
		: base(syntax)
	{
		Left = left;
		Op = op;
		Right = right;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitBinaryExpression(this);
	}
}