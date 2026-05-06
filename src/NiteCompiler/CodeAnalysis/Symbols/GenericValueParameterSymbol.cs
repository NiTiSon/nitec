namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class GenericValueParameterSymbol : Symbol
{
	public abstract int Ordinal { get; }
	public abstract TypeSymbol Type { get; }

	public sealed override SymbolKind Kind => SymbolKind.GenericValueParameter;

	public sealed override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitGenericValueParameter(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitGenericValueParameter(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitGenericValueParameter(this, arg);
	}
}