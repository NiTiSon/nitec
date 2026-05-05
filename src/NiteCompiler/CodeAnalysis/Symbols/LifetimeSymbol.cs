using System;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LifetimeSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Lifetime;

	/// <summary>
	/// Returns <see langword="true"/> when <paramref name="other"/> lifetime
	/// is always alive within <see langword="this"/> lifetime; otherwise returns <see langword="false"/>.
	/// </summary>
	public abstract bool Outlives(LifetimeSymbol other);

	public sealed override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitLifetime(this);
	}

	public sealed override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLifetime(this);
	}

	public sealed override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitLifetime(this, arg);
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		throw new InvalidOperationException();
	}
}