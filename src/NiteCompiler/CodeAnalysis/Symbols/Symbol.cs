namespace NiteCompiler.CodeAnalysis.Symbols;

internal abstract class Symbol
{
	public abstract SymbolKind Kind { get; }
}