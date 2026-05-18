namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FieldSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Field;
	public abstract TypeSymbol Type { get; }

	public sealed override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		return $"{Name}: {Type.ToDisplayString(format)}";
	}

	public sealed override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitField(this);
	}

	public sealed override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitField(this);
	}

	public sealed override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitField(this, arg);
	}
}
