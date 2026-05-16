namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class MethodSymbol : FunctionSymbol
{
	public abstract override NamedTypeSymbol ContainingSymbol { get; }
}