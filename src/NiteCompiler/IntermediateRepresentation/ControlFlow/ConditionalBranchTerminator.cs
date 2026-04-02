using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class ConditionalBranchTerminator : ControlFlowTerminator
{
	public BasicBlock Then { get; }
	public BasicBlock ElseOrMerged { get; }
	public BoundExpression Condition { get; }

	public ConditionalBranchTerminator(BoundIfStatement node, BasicBlock then, BasicBlock elseOrMerged) : base(node)
	{
		Then = then;
		ElseOrMerged = elseOrMerged;
		Condition = node.Condition;
	}
}