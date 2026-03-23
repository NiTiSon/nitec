using System.Collections.Generic;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class SimpleBlock : Block
{
	public override List<Block> Successors { get; } = [];
	public override List<Block> Predecessors { get; } = [];
	public override List<Instruction> Instructions { get; } = [];
}