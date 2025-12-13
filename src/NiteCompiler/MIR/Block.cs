using System.Collections.Generic;

namespace NiteCompiler.MIR;

public abstract class Block
{
	public readonly BlockId Id;

	protected Block(BlockId id)
	{
		Id = id;
	}

	public abstract IReadOnlyList<Block> Successors { get; }
	public abstract IReadOnlyList<Block> Predecessors { get; }
	public abstract IReadOnlyList<object> Instructions { get; }

	public bool IsUnreachable => Predecessors.Count == 0;
}