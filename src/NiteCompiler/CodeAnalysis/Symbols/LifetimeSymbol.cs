namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LifetimeSymbol : Symbol
{
	/// <summary>
	/// Returns <see langword="true"/> when <paramref name="other"/> lifetime
	/// is always alive within <see langword="this"/> lifetime; otherwise returns <see langword="false"/>.
	/// </summary>
	public abstract bool Outlives(LifetimeSymbol other);
}