using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class AddInstruction(SsaTemp output, SsaValue left, SsaValue right) : Instruction
{
	public override bool IsBranch => false;

	public readonly SsaTemp Output = output;
	public readonly SsaValue
		Left = left,
		Right = right;

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
		// writer.Write((byte)Bytecode.Add);
		// writer.Write7BitEncodedInt(output.Id);
		// writer.Write7BitEncodedInt(right.Id);
		// writer.Write7BitEncodedInt(right.Id);
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = add ");
		Left.Write(writer);
		writer.Write(", ");
		Right.Write(writer);
	}
}