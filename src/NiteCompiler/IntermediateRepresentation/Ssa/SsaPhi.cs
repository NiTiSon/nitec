using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		var keyValuePairs = Inputs.ToArray();
		for (int i = 0; i < keyValuePairs.Length; i++)
		{
			BasicBlock block = keyValuePairs.ElementAt(i).Key;
			SsaValue value = keyValuePairs.ElementAt(i).Value;

			writer.Write('[');
			writer.Write(block.Name);
			writer.Write(", ");
			value.Write(writer);
			writer.Write(']');

			if (i + 1 != keyValuePairs.Length)
			{
				writer.Write(", ");
			}
		}
	}
}