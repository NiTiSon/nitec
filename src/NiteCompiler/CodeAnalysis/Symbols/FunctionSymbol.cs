namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FunctionSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;
}