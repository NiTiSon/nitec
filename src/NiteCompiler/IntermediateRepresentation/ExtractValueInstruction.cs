using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class ExtractValueInstruction(IValue output, Operand aggregate, FieldSymbol field) : Instruction
{
	public override bool IsBranch => false;
	public IValue Output { get; } = output;
	public Operand Aggregate { get; } = aggregate;
	public FieldSymbol Field { get; } = field;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("ExtractValue instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = extractvalue ");
		Aggregate.Write(writer);
		writer.Write('.');
		writer.Write(Field.Name);
	}
}
