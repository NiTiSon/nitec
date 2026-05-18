namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class SelfParameterSymbol : ParameterSymbol
{
	public abstract override ConstructorSymbol ContainingSymbol { get; }
	public abstract override TypeSymbol ContainingType { get; }

	public sealed override string Name => "self";
	public sealed override int Ordinal => 0;

	private protected SelfParameterSymbol() {}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		return "&self";
	}
}