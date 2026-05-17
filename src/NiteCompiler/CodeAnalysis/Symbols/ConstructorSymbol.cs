namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class ConstructorSymbol : MethodSymbol
{
	public sealed override bool IsConstructor => true;
}