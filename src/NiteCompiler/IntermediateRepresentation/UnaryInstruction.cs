using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class UnaryInstruction(SsaTemp output, SsaValue input) : Instruction
{
	public sealed override bool IsBranch => false;
	protected abstract string Mnemonic { get; }

	public SsaTemp Output { get; } = output;
	public SsaValue Input { get; } = input;

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = ");
		writer.Write(Mnemonic);
		writer.Write(" ");
		Input.Write(writer);
	}
}