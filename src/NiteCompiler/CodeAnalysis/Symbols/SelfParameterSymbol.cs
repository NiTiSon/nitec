using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class SelfParameterSymbol : ParameterSymbol
{
	public override TypeSymbol ContainingSymbol => ContainingType;
	public override TypeSymbol ContainingType { get; }

	public override string Name => "self";
	public override int Ordinal => 0;

	public override TypeSymbol Type { get; }

	internal SelfParameterSymbol(TypeSymbol containingType, NiteCompilation compilation)
	{
		ContainingType = containingType;
		Type = compilation.CreateReferenceType(containingType, true, false);
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		return "&self";
	}
}