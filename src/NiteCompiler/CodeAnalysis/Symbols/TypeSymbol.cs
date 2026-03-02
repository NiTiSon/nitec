namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Type;
}