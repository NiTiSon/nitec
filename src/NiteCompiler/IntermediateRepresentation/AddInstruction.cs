using System.IO;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class AddInstruction(Value output, Value left, Value right) : Instruction
{
	public override bool IsBranch => false;

	public readonly Value
		Output = output,
		Left = left,
		Right = right;

	public override void Emit(BinaryWriter writer)
	{
		writer.Write((byte)Bytecode.Add);
		writer.Write7BitEncodedInt(output.Id);
		writer.Write7BitEncodedInt(right.Id);
		writer.Write7BitEncodedInt(right.Id);
	}
}