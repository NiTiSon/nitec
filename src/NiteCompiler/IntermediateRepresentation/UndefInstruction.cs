using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class UndefInstruction : Instruction
{
	public TempValue Output { get; }

	public UndefInstruction(TempValue output)
	{
		Output = output;
	}

	public override bool IsBranch => false;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Undef instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = undef");
	}
}
