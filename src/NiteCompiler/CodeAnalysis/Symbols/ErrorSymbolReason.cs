namespace NiteCompiler.CodeAnalysis.Symbols;

public enum ErrorSymbolReason
{
	/// <summary>
	/// Binder has more than one suitable symbols.
	/// </summary>
	Ambiguity = 0,
	/// <summary>
	/// Binder unable to find suitable symbol.
	/// </summary>
	Unknown = 1,
}