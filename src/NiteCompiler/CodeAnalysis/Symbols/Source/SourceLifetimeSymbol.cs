namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLifetimeSymbol : LifetimeSymbol
{
	public override Symbol ContainingSymbol { get; }
	public override string Name { get; }
	public override int LifetimeOrdinal { get; }

	public SourceLifetimeSymbol(Symbol containingSymbol, string name, int lifetimeOrdinal)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
		LifetimeOrdinal = lifetimeOrdinal;
	}
}