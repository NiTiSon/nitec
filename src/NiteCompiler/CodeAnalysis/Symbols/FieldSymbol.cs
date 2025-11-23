using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FieldSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Field;
	public override string Name { get; }
	public override Symbol ContainingSymbol { get; }
	public TypeSymbol Type { get; }

	private protected FieldSymbol(string name, Symbol containingSymbol, TypeSymbol type)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
		Type = type;
	}
}