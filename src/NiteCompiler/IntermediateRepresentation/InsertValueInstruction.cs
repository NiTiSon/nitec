using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class InsertValueInstruction(IValue output, Operand aggregate, Operand value, FieldSymbol field) : Instruction
{
	public override bool IsBranch => false;
	public IValue Output { get; } = output;
	public Operand Aggregate { get; } = aggregate;
	public Operand Value { get; } = value;
	public FieldSymbol Field { get; } = field;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("InsertValue instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = insertvalue ");
		Aggregate.Write(writer);
		writer.Write(", ");
		Value.Write(writer);
		writer.Write('.');
		writer.Write(Field.Name);
	}
}
