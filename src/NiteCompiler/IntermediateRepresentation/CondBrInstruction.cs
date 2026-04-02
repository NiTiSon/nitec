using System.IO;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class CondBrInstruction : Instruction
{
	public SsaValue Condition { get; }
	public BasicBlock ThenBlock { get; }
	public BasicBlock ElseBlock { get; }

	public CondBrInstruction(SsaValue condition, BasicBlock thenBlock, BasicBlock elseBlock)
	{
		Condition = condition;
		ThenBlock = thenBlock;
		ElseBlock = elseBlock;
	}

	public override bool IsBranch => true;

	public override void Emit(BinaryWriter writer)
	{
	}

	public override void Write(TextWriter writer)
	{
		writer.Write("cond br ");
		Condition.Write(writer);
		writer.Write($" {ThenBlock.Name}, {ElseBlock.Name}");
	}
}