namespace NiteCompiler.CodeAnalysis.Symbols.Metadata;

internal sealed class MetadataParameterSymbol : ParameterSymbol
{
	public override string Name { get; }
	public override int Ordinal { get; }
	public override TypeSymbol Type { get; }
	public override Symbol ContainingSymbol { get; }

	public MetadataParameterSymbol(Symbol owner, int ordinal, string name, TypeSymbol type)
	{
		ContainingSymbol = owner;
		Ordinal = ordinal;
		Name = name;
		Type = type;
	}
}
