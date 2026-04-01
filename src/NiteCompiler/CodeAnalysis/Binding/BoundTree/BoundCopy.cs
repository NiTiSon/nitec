using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundCopy(BoundExpression expression, FunctionSymbol? copyImplementation) : BoundExpression(expression.Syntax)
{
	public readonly BoundExpression Operand = expression;
	public FunctionSymbol? CopyImplementation { get; } = copyImplementation;
	public override BoundKind Kind => BoundKind.Copy;
	public override TypeSymbol Type => Operand.Type;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.LValue;

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitCopy(this);
	}
}