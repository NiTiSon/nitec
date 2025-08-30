namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class Symbol
{
	/// <summary>
	/// Gets symbol that contains this symbol or null.
	/// </summary>
	public abstract Symbol? ContainingSymbol { get; }

	/// <summary>
	/// Gets symbol type.
	/// </summary>
	public abstract SymbolKind Kind { get; }
}