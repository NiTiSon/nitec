namespace NiteCompiler.IntermediateRepresentation;

internal class LoadImmInstruction(Value output, object? constant) : Instruction
{
	public readonly Value Output = output;
	public readonly object? Constant = constant;
}