using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class LoadInstruction(TempValue output, TempValue address) : Instruction
{
	public override bool IsBranch => false;
	public TempValue Output { get; } = output;
	public TempValue Address { get; } = address;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Load instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = load ");
		Address.Write(writer);
	}
}


internal sealed class LoadParamInstruction(TempValue output, ParameterSymbol parameter) : Instruction
{
	public override bool IsBranch => false;
	public TempValue Output { get; } = output;
	public ParameterSymbol Parameter { get; } = parameter;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotSupportedException("Load instructions are not supported by the bytecode emitter yet.");
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = load param ");
		writer.Write(Parameter.ToDisplayString());
	}
}
