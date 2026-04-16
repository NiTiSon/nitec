using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class ModInstruction(SsaTemp output, SsaValue left, SsaValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "mod";

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
	}
}