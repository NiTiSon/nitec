using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class LoadIndirectInstruction(SsaTemp output, SsaValue address) : Instruction
{
	public override bool IsBranch => false;
	public SsaTemp Output { get; } = output;
	public SsaValue Address { get; } = address;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Indirect load instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = load ");
		Address.Write(writer);
	}
}
