using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaParameter(ParameterSymbol symbol) : SsaValue(symbol.Type)
{
	public ParameterSymbol Symbol { get; } = symbol;
}