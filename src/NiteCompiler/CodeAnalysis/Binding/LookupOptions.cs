namespace NiteCompiler.CodeAnalysis.Binding;

internal enum LookupOptions
{
	/// <summary>
	/// Lookup returns nothing.
	/// </summary>
	None = 0,

	/// <summary>
	/// Lookup seeks for a library.
	/// </summary>
	Libraries = 1 << 0,
	/// <summary>
	/// Lookup seeks for a module.
	/// </summary>
	Modules = 1 << 1,
	/// <summary>
	/// Lookup seeks for a type.
	/// </summary>
	Types = 1 << 2,
	/// <summary>
	/// Lookup seeks for a types' members.
	/// </summary>
	Members = 1 << 3,
	/// <summary>
	/// Lookup includes only source symbols.
	/// </summary>
	OnlySourceSymbol = 1 << 31,

	/// <summary>
	/// Default lookup options.
	/// </summary>
	Default = Types,
}