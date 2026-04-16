using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class SalInstruction(SsaTemp output, SsaValue left, SsaValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "sal";

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
	}
}