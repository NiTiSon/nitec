using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class UnaryInstruction(TempValue output, TempValue input) : Instruction
{
	public sealed override bool IsBranch => false;
	protected abstract string Mnemonic { get; }

	public TempValue Output { get; } = output;
	public TempValue Input { get; } = input;

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = ");
		writer.Write(Mnemonic);
		writer.Write(" ");
		Input.Write(writer);
	}
}