namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class LocalSymbol : Symbol, INamedSymbol
{
	public override string Name { get; }
	public TypeSymbol Type { get; }
	public override SymbolKind Kind => SymbolKind.Local;

	public LocalSymbol(string name, TypeSymbol type)
	{
		Name = name;
		Type = type;
	}
}