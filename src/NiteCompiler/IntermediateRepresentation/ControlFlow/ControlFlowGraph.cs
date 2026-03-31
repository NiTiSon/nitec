using System;
using System.Diagnostics;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class ControlFlowGraph
{
	public BasicBlock[] BasicBlocks { get; }
	public BasicBlock Entry { get; }

	public ControlFlowGraph(BasicBlock entry, BasicBlock[] blocks, BasicBlock[] basicBlocks)
	{
		Debug.Assert(blocks.Contains(entry));

		BasicBlocks = basicBlocks;
		Entry = entry;
	}
}