namespace NiteCompiler.CodeAnalysis.Symbols;

/// <summary>
/// Variable symbol represent symbol which value lifetime is only within function bounds.
/// </summary>
public abstract class LocalVariableOrParameterSymbol : Symbol
{
	public abstract TypeSymbol Type { get; }

	private protected LocalVariableOrParameterSymbol() {}
}