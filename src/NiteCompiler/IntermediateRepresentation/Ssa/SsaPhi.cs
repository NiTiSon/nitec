using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaPhi
{
	public LocalVariableOrParameterSymbol Variable { get; }
	public SsaTemp? Result { get; set; }
	public Dictionary<BasicBlock, SsaValue> Inputs { get; } = [];
	public TypeSymbol Type => Variable.Type;

	public SsaPhi(LocalVariableOrParameterSymbol variable)
	{
		Variable = variable;
	}

	public void Write(TextWriter writer)
	{
		Debug.Assert(Result != null);

		Result.Write(writer);
		writer.Write($" = phi ");
		foreach (var (block, value) in Inputs)
		{
			writer.Write('[');
			writer.Write(block.Id);
			writer.Write(", ");
			value.Write(writer);
			writer.Write(']');
		}
		writer.Write(';');
	}
}