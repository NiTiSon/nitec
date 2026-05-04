using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class StoreLocalAddressInstruction(LocalVariableOrParameterSymbol symbol, SsaValue value) : Instruction
{
	public override bool IsBranch => false;
	public LocalVariableOrParameterSymbol Symbol { get; } = symbol;
	public SsaValue Value { get; } = value;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Address-backed local stores are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		writer.Write("store local ");
		Value.Write(writer);
		writer.Write(", ");
		writer.Write(Symbol.Name);
	}
}
