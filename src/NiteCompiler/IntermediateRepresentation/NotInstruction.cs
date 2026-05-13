using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class NotInstruction(TempValue output, TempValue input) : UnaryInstruction(output, input)
{
	protected override string Mnemonic => "not";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}