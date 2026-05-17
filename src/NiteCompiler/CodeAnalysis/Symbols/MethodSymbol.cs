namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class MethodSymbol : FunctionSymbol
{
	public abstract override TypeSymbol ContainingSymbol { get; }
	public abstract SelfParameterSymbol SelfParameter { get; }

	public sealed override bool IsMethod => true;
}