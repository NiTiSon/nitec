using System;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LifetimeParameterSymbol : Symbol
{
	public abstract int LifetimeOrdinal { get; }
	public sealed override SymbolKind Kind => SymbolKind.LifetimeParameter;

	public sealed override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitLifetimeParameter(this);
	}

	public sealed override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLifetimeParameter(this);
	}

	public sealed override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitLifetimeParameter(this, arg);
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		return "'" + Name;
	}
}