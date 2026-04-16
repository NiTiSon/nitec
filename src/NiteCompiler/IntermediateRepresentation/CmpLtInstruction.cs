using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class CmpLtInstruction(SsaTemp output, SsaValue left, SsaValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "cmp lt";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}