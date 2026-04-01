using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

[DebuggerDisplay("BasicBlock {Id} in({Predecessors.Count}) out({Successors.Count})")]
internal sealed class BasicBlock(int id)
{
	public int Id { get; } = id;
	public List<BoundStatement> Statements { get; } = [];
	public ControlFlowTerminator? Terminator { get; internal set; }

	public List<BasicBlock> Predecessors { get; } = [];
	public List<BasicBlock> Successors { get; } = [];
}