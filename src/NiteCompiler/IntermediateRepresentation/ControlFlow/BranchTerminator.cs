using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class BranchTerminator : ControlFlowTerminator
{
	public BasicBlock Target { get; }

	public BranchTerminator(BoundNode? node, BasicBlock target) : base(node)
	{
		Target = target;
	}
}