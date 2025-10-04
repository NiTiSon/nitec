namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : Symbol
{
	public override SymbolKind Kind => SymbolKind.Type;
}