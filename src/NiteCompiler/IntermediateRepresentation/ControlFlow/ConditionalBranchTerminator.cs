using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class ConditionalBranchTerminator : ControlFlowTerminator
{
	public BasicBlock Then { get; }
	public BasicBlock Else { get; }
	public BoundExpression Condition { get; }

	public ConditionalBranchTerminator(BoundExpression condition, BasicBlock then, BasicBlock @else) : base(condition)
	{
		Then = then;
		Else = @else;
		Condition = condition;
	}
}