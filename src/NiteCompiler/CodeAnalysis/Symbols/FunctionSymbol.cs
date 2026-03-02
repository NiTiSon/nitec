namespace NiteCompiler.CodeAnalysis.Symbols;

internal abstract class FunctionSymbol : Symbol
{
	public override SymbolKind Kind => SymbolKind.Function;
}