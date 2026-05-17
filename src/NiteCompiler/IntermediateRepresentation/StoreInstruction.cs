using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class StoreInstruction(Operand value, Operand address) : Instruction
{
	public override bool IsBranch => false;
	public Operand Value { get; } = value;
	public Operand Address { get; } = address;

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
