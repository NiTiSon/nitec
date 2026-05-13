using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class CmpGeInstruction(TempValue output, TempValue left, TempValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "cmp ge";

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
	}
}