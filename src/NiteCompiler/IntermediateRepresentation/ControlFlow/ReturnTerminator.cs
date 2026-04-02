using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class ReturnTerminator : ControlFlowTerminator
{
	public BoundExpression? Expression { get; }

	public ReturnTerminator(BoundReturn? node) : base(node)
	{
		Expression = node?.Expression;
	}
}