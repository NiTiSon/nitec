namespace NiteCompiler.IntermediateRepresentation;

internal sealed class RetInstruction(Value? value) : Instruction
{
	public Value? Value { get; } = value;
}