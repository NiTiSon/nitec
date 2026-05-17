using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class CmpEqInstruction(IValue output, Operand left, Operand right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "cmp eq";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}