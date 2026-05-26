using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class UIToFPInstruction(IValue output, Operand input) : UnaryInstruction(output, input)
{
	protected override string Mnemonic => "uitofp";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}
