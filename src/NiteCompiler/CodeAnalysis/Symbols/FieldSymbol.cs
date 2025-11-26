using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FieldSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Field;
	public abstract override string Name { get; }
	public abstract override Symbol ContainingSymbol { get; }
	public abstract TypeSymbol Type { get; }

	private protected FieldSymbol() {}
}