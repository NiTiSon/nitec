using System;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LifetimeSymbol : Symbol
{
	public abstract int LifetimeOrdinal { get; }
	public sealed override SymbolKind Kind => SymbolKind.Lifetime;

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
		return '\'' + Name;
	}
}