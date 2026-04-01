using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaTemp(TypeSymbol type, int id) : SsaValue(type)
{
	public int Id { get; } = id;
}