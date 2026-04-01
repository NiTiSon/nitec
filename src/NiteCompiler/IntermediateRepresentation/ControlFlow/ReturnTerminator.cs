using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class ReturnTerminator : ControlFlowTerminator
{
	public ReturnTerminator(BoundReturn? node) : base(node)
	{

	}
}