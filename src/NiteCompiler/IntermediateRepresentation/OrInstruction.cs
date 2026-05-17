using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class OrInstruction(IValue output, Operand left, Operand right) : BinaryInstruction(output, left, right)
{
	protected override string Mnemonic => "or";

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
	}
}