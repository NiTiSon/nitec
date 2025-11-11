namespace NiteCompiler.CodeAnalysis.Binding;

internal enum LookupOptions
{
	None = 0,

	Libraries = 1 << 0,
	Modules = 1 << 1,
	SymbolsFromThisLibrary = 1 << 31,

	Default = Modules | SymbolsFromThisLibrary,
}