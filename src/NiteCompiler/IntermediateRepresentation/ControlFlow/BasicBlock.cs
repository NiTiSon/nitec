using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class BasicBlock(int id)
{
	public int Id { get; } = id;
	public List<BoundStatement> Statements { get; } = [];
	public BoundStatement? Terminator { get; internal set; }

	public List<BasicBlock> Predecessors { get; } = [];
	public List<BasicBlock> Successors { get; } = [];
}