using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class ModuleSymbol : Symbol
{
	public abstract override string Name { get; }
	public sealed override SymbolKind Kind => SymbolKind.Module;
	public abstract ImmutableArray<Symbol> Members { get; }
	public abstract override LibrarySymbol? ContainingSymbol { get; }

	private protected ModuleSymbol() {}
}