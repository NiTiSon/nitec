namespace NiteCompiler.CodeAnalysis.Symbols;

public enum SymbolKind : ushort
{
	/// <summary>
	/// Symbol is alias to other symbol.
	/// </summary>
	Alias = 0,

	/// <summary>
	/// Symbol is a library.
	/// </summary>
	Library = 1,

	/// <summary>
	/// Symbol is a module.
	/// </summary>
	Module = 2,

	/// <summary>
	/// Symbol is a function.
	/// </summary>
	Function = 3,

	/// <summary>
	/// Symbol is a type.
	/// </summary>
	Type = 4,

	/// <summary>
	/// Symbol is a function parameter.
	/// </summary>
	Parameter = 5,

	/// <summary>
	/// Symbol is a generic parameter.
	/// </summary>
	GenericParameter = 6,

	/// <summary>
	/// Symbol is a field.
	/// </summary>
	Field = 7,

	/// <summary>
	/// Symbol is local variable.
	/// </summary>
	Local = 8,
}