namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class StaticLifetimeSymbol : LifetimeSymbol
{
	public static readonly StaticLifetimeSymbol Instance = new();

	public override Symbol ContainingSymbol => null!;
	public override string Name => "static";
	public override bool IsStaticLifetime => true;
	public override int LifetimeOrdinal => -1;

	private StaticLifetimeSymbol() { }
}
