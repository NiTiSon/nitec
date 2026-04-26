using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundCompoundAssignment : BoundExpression
{
	public BoundExpression Left { get; }
	public BinaryOperatorSignature Op { get; }
	public BoundExpression Right { get; }

	public override BoundKind Kind => BoundKind.CompoundAssignmentExpression;


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
	public BoundCompoundAssignment(SyntaxNode syntax, BinaryOperatorSignature op,
		BoundExpression left, BoundExpression right, bool hasErrors = false) : base(syntax, hasErrors)
	{
		Op = op;
		Left = left;
		Right = right;
	}


	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitCompoundAssignment(this);
	}

	public override TypeSymbol Type => Op.ReturnType;
	public override Binder.BindValueKind ValueKind =>  Binder.BindValueKind.RValue;
}