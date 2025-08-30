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
	Package = 1,

	/// <summary>
	/// Symbol is a module.
	/// </summary>
	Module = 2,

	/// <summary>
	/// Symbol is function.
	/// </summary>
	Function = 3,
}