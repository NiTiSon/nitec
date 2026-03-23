using System.Collections.Generic;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class Block
{
	public abstract IReadOnlyList<Block> Successors { get; }
	public abstract IReadOnlyList<Block> Predecessors { get; }
	public abstract IReadOnlyList<Instruction> Instructions { get; }

	public bool IsUnreachable => Predecessors.Count == 0;
}