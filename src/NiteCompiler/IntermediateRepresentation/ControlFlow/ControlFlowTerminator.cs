using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal abstract class ControlFlowTerminator
{
	public BoundNode? Node { get; }

	protected ControlFlowTerminator(BoundNode? node)
	{
		Node = node;
	}
}