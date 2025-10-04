namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class NamedTypeSymbol : TypeSymbol
{
	public string Name { get; }

	public NamedTypeSymbol(string name, Symbol containingSymbol)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
	}

	public override Symbol? ContainingSymbol { get; }
}