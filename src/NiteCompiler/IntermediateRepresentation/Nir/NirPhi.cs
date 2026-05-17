using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class NirPhi(LocalVariableOrParameterSymbol variable)
{
	public LocalVariableOrParameterSymbol Variable { get; } = variable;
	public Temp? Result { get; set; }
	public Dictionary<BasicBlock, IValue> Inputs { get; } = new();
}
