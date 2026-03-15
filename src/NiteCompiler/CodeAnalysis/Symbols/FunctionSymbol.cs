namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FunctionSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;

	public override string ToDisplayString()
	{
		string separator = (ContainingSymbol is TypeSymbol && !IsStatic) ? "." : "::";
		return $"{ContainingSymbol!.ToDisplayString()}{separator}{Name}";
	}
}