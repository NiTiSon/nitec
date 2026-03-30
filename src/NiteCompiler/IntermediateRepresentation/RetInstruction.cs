using System.IO;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class RetInstruction(Value? value) : Instruction
{
	public override bool IsBranch => true;

	public Value? Value { get; } = value;

	public override void Emit(BinaryWriter writer)
	{
		if (Value == null)
		{
			writer.Write((byte)Bytecode.RetVoid);
		}
		else
		{
			writer.Write((byte)Bytecode.Ret);
			writer.Write7BitEncodedInt(Value.Id);
		}
	}
}