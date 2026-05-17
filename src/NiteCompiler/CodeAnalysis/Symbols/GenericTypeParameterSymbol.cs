namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class GenericTypeParameterSymbol : TypeSymbol
{
	public sealed override TypeKind TypeKind => TypeKind.TypeParameter;
	public abstract int Ordinal { get; }

	public sealed override SymbolKind Kind => SymbolKind.GenericTypeParameter;

	public virtual GenericTypeParameterSymbol OriginalDefinition => this;

	public sealed override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitGenericTypeParameter(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitGenericTypeParameter(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitGenericTypeParameter(this, arg);
	}
}