using System.IO;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class UndefInstruction : Instruction
{
	public IValue Output { get; }

	public UndefInstruction(IValue output)
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
