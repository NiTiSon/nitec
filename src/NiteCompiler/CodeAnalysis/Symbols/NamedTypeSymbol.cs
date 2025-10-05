namespace NiteCompiler.CodeAnalysis.Symbols;

// TODO: Implement
public sealed class NamedTypeSymbol : TypeSymbol
{
	public override string Name { get; }

	public NamedTypeSymbol(string name, Symbol containingSymbol)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
	}

	public override Symbol? ContainingSymbol { get; }
}