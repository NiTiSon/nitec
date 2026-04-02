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

	public override void Write(TextWriter writer)
	{
		if (Value == null)
		{
			writer.Write("ret void");
		}
		else
		{
			writer.Write("ret ");
			Value.Write(writer);
		}
	}
}