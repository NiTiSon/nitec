using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class CmpGeInstruction(SsaTemp output, SsaValue left, SsaValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "cmp ge";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}