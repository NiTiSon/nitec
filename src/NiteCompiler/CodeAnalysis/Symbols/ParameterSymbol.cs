namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class ParameterSymbol : LocalVariableOrParameterSymbol
{
	public override SymbolKind Kind => SymbolKind.Parameter;
	public abstract int Ordinal { get; }
	public abstract override TypeSymbol Type { get; }

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitParameter(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitParameter(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitParameter(this, arg);
	}

	public override string ToDisplayString()
	{
		return Name;
	}
}