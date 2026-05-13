using System.Collections.Generic;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Mir;

internal sealed class MirBlock(BasicBlock controlFlowBlock)
{
	public BasicBlock ControlFlowBlock { get; } = controlFlowBlock;
	public List<Instruction> Instructions { get; } = [];
}