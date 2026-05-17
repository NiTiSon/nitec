namespace NiteCompiler.CodeAnalysis.Symbols;

public enum SymbolKind
{
	None = 0,
	Library,
	Module,
	NamedType,
	ErrorType,
	Field,
	Property,
	Function,
	Parameter,
	GenericTypeParameter,
	GenericValueParameter,
	Lifetime,
	LocalVariable,
	Pointer,
	Reference,
	Array,
}