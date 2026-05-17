using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class BinaryInstruction(IValue output, Operand left, Operand right) : Instruction
{
	public sealed override bool IsBranch => false;
	protected abstract string Mnemonic { get; }

	public IValue Output { get; } = output;
	public Operand Left { get; } = left;
	public Operand Right { get; } = right;

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