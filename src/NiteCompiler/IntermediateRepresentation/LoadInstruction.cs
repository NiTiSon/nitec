using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class LoadInstruction(IValue output, Operand address) : Instruction
{
	public override bool IsBranch => false;
	public IValue Output { get; } = output;
	public Operand Address { get; } = address;

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


internal sealed class LoadParamInstruction(IValue output, ParameterSymbol parameter) : Instruction
{
	public override bool IsBranch => false;
	public IValue Output { get; } = output;
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
