using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class FPToUIInstruction(IValue output, Operand input) : UnaryInstruction(output, input)
{
	protected override string Mnemonic => "fptoui";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}
