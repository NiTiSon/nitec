namespace NiteCode.Compiler.CodeAnalysis.Symbols;

public abstract class Symbol
{
	private protected Symbol()
	{

	}
	public abstract SymbolKind Kind { get; }
}