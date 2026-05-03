namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class NamedTypeSymbol : TypeSymbol
{
	/// <summary>
	/// The user-defined type name.
	/// </summary>
	public abstract override string Name { get; }

	/// <summary>
	/// The amount of generic arguments.
	/// </summary>
	public abstract override int Arity { get; }
}