using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class GetElementPointer(IValue output, Operand baseAddress, FieldSymbol field) : Instruction
{
	public override bool IsBranch => false;
	public IValue Output { get; } = output;
	public Operand BaseAddress { get; } = baseAddress;
	public FieldSymbol Field { get; } = field;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("GetElementPointer instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = gep ");
		BaseAddress.Write(writer);
		writer.Write('.');
		writer.Write(Field.Name);
	}
}
