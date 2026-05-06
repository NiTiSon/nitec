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
	GenericTypeParameter = 8,
	GenericValueParameter = 8,
	LifetimeParameter = 9,
	LocalVariable = 11,
}