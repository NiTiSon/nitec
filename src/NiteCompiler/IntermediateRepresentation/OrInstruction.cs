using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class OrInstruction(TempValue output, TempValue left, TempValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "or";

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
	}
}