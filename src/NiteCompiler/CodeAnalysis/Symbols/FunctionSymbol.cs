using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FunctionSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;

	public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

	public override string ToDisplayString()
	{
		string separator = (ContainingSymbol is TypeSymbol && !IsStatic) ? "." : "::";
		return $"{ContainingSymbol!.ToDisplayString()}{separator}{Name}";
	}
}