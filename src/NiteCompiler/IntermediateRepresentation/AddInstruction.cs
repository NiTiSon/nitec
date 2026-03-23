namespace NiteCompiler.IntermediateRepresentation;

internal sealed class AddInstruction(Value output, Value left, Value right) : Instruction
{
	public readonly Value
		Output = output,
		Left = left,
		Right = right;
}