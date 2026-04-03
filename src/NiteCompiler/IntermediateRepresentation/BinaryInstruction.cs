using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class BinaryInstruction(SsaTemp output, SsaValue left, SsaValue right) : Instruction
{
	public sealed override bool IsBranch => false;
	protected abstract string Mnemonic { get; }

	public SsaTemp Output { get; } = output;
	public SsaValue Left { get; } = left;
	public SsaValue Right { get; } = right;

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = ");
		writer.Write(Mnemonic);
		writer.Write(" ");
		Left.Write(writer);
		writer.Write(" ");
		Right.Write(writer);
	}
}