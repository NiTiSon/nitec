using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class NotInstruction(IValue output, Operand input) : UnaryInstruction(output, input)
{
	protected override string Mnemonic => "not";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}