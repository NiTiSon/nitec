using System.IO;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class BrInstruction : Instruction
{
	public BasicBlock Target { get; }

	public override bool IsBranch => true;

	public BrInstruction(BasicBlock target)
	{
		Target = target;
	}

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}

	public override void Write(TextWriter writer)
	{
		writer.Write("br ");
		writer.Write($"{Target.Name}");
	}
}