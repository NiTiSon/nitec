namespace NiteCompiler.CodeAnalysis.Symbols;

internal abstract class LibrarySymbol : Symbol
{
	public override SymbolKind Kind => SymbolKind.Library;
	public override Symbol? ContainingSymbol => null;
	public override LibrarySymbol? ContainingLibrary => null;
}