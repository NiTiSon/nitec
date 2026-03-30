using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal sealed class BoundCopy(BoundExpression expression) : BoundExpression(expression.Syntax)
{
	public readonly BoundExpression Operand = expression;
	public override BoundKind Kind => BoundKind.Copy;
	public override TypeSymbol Type => Operand.Type;
	public override Binder.BindValueKind ValueKind => Operand.ValueKind;
}