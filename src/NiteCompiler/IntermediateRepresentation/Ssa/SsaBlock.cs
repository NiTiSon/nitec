using System.Collections.Generic;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaBlock
{
	public BasicBlock ControlFlowBlock { get; }
	public List<SsaPhi> Phis { get; } = [];
	public List<Instruction> Instructions { get; } = [];

	public SsaBlock(BasicBlock controlFlowBlock)
	{
		ControlFlowBlock = controlFlowBlock;
	}
}