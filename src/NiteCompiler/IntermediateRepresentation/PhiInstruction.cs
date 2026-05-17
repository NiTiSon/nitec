using System.Collections.Generic;
using System.IO;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class PhiInstruction : Instruction
{
	public IValue Output { get; }
	public List<(Operand Value, BasicBlock Block)> Incoming { get; } = [];

	public PhiInstruction(IValue output)
	{
		Output = output;
	}

	public override bool IsBranch => false;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Phi instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = phi [");
		for (int i = 0; i < Incoming.Count; i++)
		{
			if (i > 0) writer.Write(", ");
			Incoming[i].Value.Write(writer);
			writer.Write(" from ");
			writer.Write(Incoming[i].Block.Name);
		}
		writer.Write("]");
	}
}
