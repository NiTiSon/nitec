using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FunctionSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;
	public override Symbol ContainingSymbol { get; }
	public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

	private protected FunctionSymbol(Symbol containingSymbol)
	{
		ContainingSymbol = containingSymbol;
	}
}