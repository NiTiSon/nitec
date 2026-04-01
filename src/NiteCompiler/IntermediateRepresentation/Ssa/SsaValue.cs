using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal abstract class SsaValue
{
	public TypeSymbol Type { get; }

	protected SsaValue(TypeSymbol type)
	{
		Type = type;
	}
}