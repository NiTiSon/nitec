using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class AddInstruction(SsaTemp output, SsaValue left, SsaValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "add";

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
		// writer.Write((byte)Bytecode.Add);
		// writer.Write7BitEncodedInt(output.Id);
		// writer.Write7BitEncodedInt(right.Id);
		// writer.Write7BitEncodedInt(right.Id);
	}
}