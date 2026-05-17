using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class AddressOfInstruction(IValue output, LocalVariableOrParameterSymbol symbol) : Instruction
{
	public override bool IsBranch => false;
	public IValue Output { get; } = output;
	public LocalVariableOrParameterSymbol Symbol { get; } = symbol;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Address-of instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = address of ");
		writer.Write(Symbol.Name);
	}
}
