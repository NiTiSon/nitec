namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ParameterSymbol : Symbol
{
	public override string Name { get; }
	public override SymbolKind Kind => SymbolKind.Parameter;
	public override Symbol? ContainingSymbol { get; }
	public TypeSymbol Type { get; }

	public ParameterSymbol(string name, Symbol containingSymbol, TypeSymbol type)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
		Type = type;
	}
}