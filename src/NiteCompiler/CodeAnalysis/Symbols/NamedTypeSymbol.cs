namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class NamedTypeSymbol : TypeSymbol
{
	public override TypeKind TypeKind => TypeKind.SimpleType;
	public sealed override SymbolKind Kind => SymbolKind.NamedType;

	/// <summary>
	/// The user-defined type name.
	/// </summary>
	public abstract override string Name { get; }

	/// <summary>
	/// The amount of lifetime arguments.
	/// </summary>
	public abstract override int LifetimeArity { get; }

	/// <summary>
	/// The amount of generic arguments.
	/// </summary>
	public abstract override int Arity { get; }
}