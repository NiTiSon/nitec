namespace NiteCompiler.CodeAnalysis.Symbols;

public enum SymbolKind
{
	None = 0,
	Library = 1,
	Module = 2,
	Type = 3,
	Field = 4,
	Property = 5,
	Function = 6,
	Parameter = 7,
	GenericParameter = 8,
	LifetimeParameter = 9,
	LocalVariable = 10,
}