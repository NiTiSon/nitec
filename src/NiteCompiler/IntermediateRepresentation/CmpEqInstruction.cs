using System.IO;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class CmpEqInstruction(TempValue output, TempValue left, TempValue right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "cmp eq";

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}
}