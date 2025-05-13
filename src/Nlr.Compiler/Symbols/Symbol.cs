namespace Nlr.Compiler.Symbols;

public abstract class Symbol
{
	public abstract string Name { get; }

	public abstract ModuleSymbol ModuleContainer { get; }
	
	public abstract SymbolKind Kind { get; }
}