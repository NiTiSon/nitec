using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FunctionSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;
	public override string Name { get; }
	public override Symbol ContainingSymbol { get; }
	public ImmutableArray<ParameterSymbol> Parameters { get; }
	public bool IsMethod => ContainingSymbol is TypeSymbol;

	private protected FunctionSymbol(string name, Symbol containingSymbol, ImmutableArray<ParameterSymbol> parameters)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
		Parameters = parameters;
	}
}