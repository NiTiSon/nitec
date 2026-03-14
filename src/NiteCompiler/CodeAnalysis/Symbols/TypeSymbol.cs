namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : ContainerSymbol
{
	public sealed override SymbolKind Kind => SymbolKind.Type;

	public abstract SpecialType SpecialType { get; }
}