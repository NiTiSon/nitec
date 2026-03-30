using System.Collections.Generic;
using System.Linq;

namespace NiteCompiler.IntermediateRepresentation;

// TODO: Make poolable
internal sealed class SimpleBlock : Block
{
	public override BlockId Id { get; }
	public override List<Block> Successors { get; } = [];
	public override List<Block> Predecessors { get; } = [];
	public override List<Instruction> Instructions { get; } = [];

	public SimpleBlock(BlockId id)
	{
		Id = id;
	}

	public bool HasBranchInstruction => Instructions.Any(t => t.IsBranch);
	public bool HasBranchInstructionAtTheEnd => Instructions.LastOrDefault()?.IsBranch ?? false;
}