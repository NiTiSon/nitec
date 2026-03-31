using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundMove(BoundExpression expression) : BoundExpression(expression.Syntax)
{
	public readonly BoundExpression Operand;
	public override BoundKind Kind => BoundKind.Move;
	public override TypeSymbol Type => Operand.Type;
	public override Binder.BindValueKind ValueKind => Operand.ValueKind;

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitMove(this);
	}
}