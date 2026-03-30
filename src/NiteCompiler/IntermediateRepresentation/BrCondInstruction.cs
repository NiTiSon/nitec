using System.IO;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class BrCondInstruction : Instruction
{
	public Value Condition { get; }
	public BlockId Then { get; }
	public BlockId Else { get; }

	public override bool IsBranch => true;

	public BrCondInstruction(Value condition, BlockId then, BlockId @else)
	{
		Condition = condition;
		Then = then;
		Else = @else;
	}

	public override void Emit(BinaryWriter writer)
	{
		writer.Write((byte)Bytecode.BrCond);
		writer.Write7BitEncodedInt(Condition.Id);
		writer.Write7BitEncodedInt(Then.Id);
		writer.Write7BitEncodedInt(Else.Id);
	}
}