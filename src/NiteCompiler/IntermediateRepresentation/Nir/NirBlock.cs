using System.Collections.Generic;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class NirBlock(BasicBlock controlFlowBlock)
{
	public BasicBlock ControlFlowBlock { get; } = controlFlowBlock;
	public List<NirPhi> Phis { get; } = new();
	public List<Instruction> Instructions { get; } = new();
}
