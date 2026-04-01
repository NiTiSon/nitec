using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class RetInstruction(SsaValue? value) : Instruction
{
	public override bool IsBranch => true;

	public SsaValue? Value { get; } = value;

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
		// if (Value == null)
		// {
		// 	writer.Write((byte)Bytecode.RetVoid);
		// }
		// else
		// {
		// 	writer.Write((byte)Bytecode.Ret);
		// 	writer.Write7BitEncodedInt(Value.Id);
		// }
	}
}