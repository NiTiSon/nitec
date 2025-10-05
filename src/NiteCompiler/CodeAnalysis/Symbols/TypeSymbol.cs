namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : Symbol
{
	public abstract string Name { get; }
	public override SymbolKind Kind => SymbolKind.Type;
}