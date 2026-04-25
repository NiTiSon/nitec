using System;
using System.IO;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class DropInstruction(SsaValue value) : Instruction
{
	public SsaValue Value { get; } = value;
	public override bool IsBranch => false;

	public override void Emit(BinaryWriter writer)
	{
		throw new NotImplementedException();
	}

	public override void Write(TextWriter writer)
	{
		writer.Write("drop ");
		Value.Write(writer);
	}
}