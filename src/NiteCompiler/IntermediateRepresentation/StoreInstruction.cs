using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class StoreInstruction(TempValue value, TempValue address) : Instruction
{
	public override bool IsBranch => false;
	public TempValue Value { get; } = value;
	public TempValue Address { get; } = address;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Indirect store instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		writer.Write("store ");
		Value.Write(writer);
		writer.Write(", ");
		Address.Write(writer);
	}
}
