using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class ShrInstruction(TempValue output, TempValue left, TempValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "shr";

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
	}
}