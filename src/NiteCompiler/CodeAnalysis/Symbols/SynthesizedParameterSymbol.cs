namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class SynthesizedParameterSymbol : ParameterSymbol
{
	public override string Name { get; }
	public override TypeSymbol Type { get; }
	public override Symbol ContainingSymbol { get; }
	public override int Ordinal { get; }

	public SynthesizedParameterSymbol(Symbol containingSymbol, string name, TypeSymbol type, int ordinal)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
		Type = type;
		Ordinal = ordinal;
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		if (format.HasFlag(SymbolFormat.OmitParameterNames))
		{
			return Type.ToDisplayString(format);
		}

		return $"{Name}: {Type.ToDisplayString(format)}";
	}

	public override int LifetimeArity => 0;
	public override int Arity => 0;
}
