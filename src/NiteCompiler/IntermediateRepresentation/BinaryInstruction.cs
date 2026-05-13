using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class BinaryInstruction(TempValue output, TempValue left, TempValue right) : Instruction
{
	public sealed override bool IsBranch => false;
	protected abstract string Mnemonic { get; }

	public TempValue Output { get; } = output;
	public TempValue Left { get; } = left;
	public TempValue Right { get; } = right;

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