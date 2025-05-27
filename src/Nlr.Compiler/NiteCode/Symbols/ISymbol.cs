namespace Nlr.Compiler.NiteCode.Symbols;

public interface ISymbol
{
	string Name { get; }

	ModuleSymbol ModuleContainer { get; }
	
	SymbolKind Kind { get; }
}