using System.Collections.Immutable;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FunctionSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;

	public abstract TypeSymbol ReturnType { get; }
	public abstract ImmutableArray<ParameterSymbol> Parameters { get; }


	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		string separator = (ContainingSymbol is TypeSymbol && !IsStatic) ? "." : "::";
		string result = $"{ContainingSymbol!.ToDisplayString()}{separator}{Name}";

		result += $"({string.Join(", ", Parameters.Select(t => t.ToDisplayString(format)))})";
		result += $" -> {ReturnType.ToDisplayString(format)}";
		return result;
	}
}