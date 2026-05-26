using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class ZExtInstruction(IValue output, Operand input) : UnaryInstruction(output, input)
{
	protected override string Mnemonic => "zext";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}
