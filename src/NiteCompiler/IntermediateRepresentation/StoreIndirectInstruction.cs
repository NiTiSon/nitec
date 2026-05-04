using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class StoreIndirectInstruction(SsaValue address, SsaValue value) : Instruction
{
	public override bool IsBranch => false;
	public SsaValue Address { get; } = address;
	public SsaValue Value { get; } = value;

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
