using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaFunction(FunctionSymbol function)
{
	public FunctionSymbol Function { get; } = function;
	public Dictionary<BasicBlock, SsaBlock> Blocks { get; } = new();
}