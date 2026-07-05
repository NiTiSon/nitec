using System.Collections.Immutable;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FunctionSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;

	public abstract TypeSymbol? ReturnType { get; }
	public abstract ImmutableArray<LifetimeSymbol> Lifetimes { get; }
	public abstract ImmutableArray<LifetimeConstraint> LifetimeConstraints { get; }
	public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

	public sealed override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitFunction(this);
	}

	public sealed override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitFunction(this);
	}

	public sealed override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitFunction(this, arg);
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		string separator = (ContainingSymbol is TypeSymbol && !IsStatic) ? "." : "::";
		string result = $"{ContainingSymbol!.ToDisplayString(format)}{separator}{Name}";

		result += $"({string.Join(", ", Parameters.Select(t => t.ToDisplayString(format)))})";
		if (!format.HasFlag(SymbolFormat.OmitReturnType))
		{
			result += ReturnType != null ? $" -> {ReturnType.ToDisplayString(format)}" : "";
		}
		return result;
	}
}