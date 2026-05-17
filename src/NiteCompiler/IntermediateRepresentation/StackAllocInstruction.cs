using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class StackAllocInstruction(IValue output, TypeSymbol typeOf, int amount = 1) : Instruction
{
	public IValue Output { get; } = output;
	public TypeSymbol TypeOf { get; } = typeOf;
	public int Amount { get; } = amount;

	public override bool IsBranch => false;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = stackalloc ");
		writer.Write(TypeOf.ToDisplayString());
		if (Amount > 1)
		{
			writer.Write(", amount ");
			writer.Write(Amount);
		}
	}
}