using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundMove(BoundExpression expression) : BoundExpression(expression.Syntax)
{
	public readonly BoundExpression Operand = expression;
	public override BoundKind Kind => BoundKind.Move;
	public override TypeSymbol Type => Operand.Type;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;
	public override Pureness Pureness => Operand.Pureness;

	public override void Accept(BoundVisitor visitor) => visitor.VisitMove(this);
}