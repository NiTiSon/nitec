using System.Collections.Immutable;
using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class CallInstruction(TempValue output, FunctionSymbol function, ImmutableArray<TempValue> arguments) : Instruction
{
	public TempValue Output { get; } = output;
	public FunctionSymbol Function { get; } = function;
	public ImmutableArray<TempValue> Arguments { get; } = arguments;

	public override bool IsBranch => true;

	public override void Emit(BinaryWriter writer)
	{
		throw new System.NotImplementedException();
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = call ");
		writer.Write(Function.ToDisplayString(SymbolFormat.Detailed | SymbolFormat.OmitParameterNames));
		writer.Write(" [");
		for (int i = 0; i < Arguments.Length; i++)
		{
			TempValue value = Arguments[i];

			value.Write(writer);

			if (i + 1 != Arguments.Length)
			{
				writer.Write(", ");
			}
		}
		writer.Write("]");
	}
}