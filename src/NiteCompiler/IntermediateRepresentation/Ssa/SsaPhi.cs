using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaPhi : SsaInstruction
{
	public LocalVariableOrParameterSymbol Variable { get; }
	public SsaTemp? Result { get; set; }
	public Dictionary<BasicBlock, SsaValue> Inputs { get; } = [];
	public TypeSymbol Type => Variable.Type;

	public SsaPhi(LocalVariableOrParameterSymbol variable)
	{
		Variable = variable;
	}
}