using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Mir;

internal sealed class FunctionMir(FunctionSymbol function)
{
	public FunctionSymbol Function { get; } = function;
	public Dictionary<BasicBlock, MirBlock> Blocks { get; } = new();
	public Dictionary<LocalVariableOrParameterSymbol, TempValue>? AddressTable { get; set; }
}