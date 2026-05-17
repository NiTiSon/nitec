using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class UnaryInstruction(IValue output, Operand input) : Instruction
{
	public sealed override bool IsBranch => false;
	protected abstract string Mnemonic { get; }

	public IValue Output { get; } = output;
	public Operand Input { get; } = input;

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = ");
		writer.Write(Mnemonic);
		writer.Write(" ");
		Input.Write(writer);
	}
}