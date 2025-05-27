namespace Nlr.Compiler.NiteCode.Symbols;

public interface INamedSymbol : ISymbol
{
	string Name { get; }
}