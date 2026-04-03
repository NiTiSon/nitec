using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class NegInstruction(SsaTemp output, SsaValue input) : UnaryInstruction(output, input)
{
	protected override string Mnemonic => "neg";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}