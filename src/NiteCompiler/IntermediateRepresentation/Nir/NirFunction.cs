using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class NirFunction(FunctionSymbol function)
{
	public FunctionSymbol Function { get; } = function;
	public Dictionary<BasicBlock, NirBlock> Blocks { get; } = new();
	public Dictionary<ParameterSymbol, Param> Parameters { get; } = new();
}
