namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LocalVariableSymbol : Symbol
{
	public override SymbolKind Kind => SymbolKind.LocalVariable;
	public abstract TypeSymbol Type { get; }

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitLocalVariable(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLocalVariable(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitLocalVariable(this, arg);
	}

	public override string ToDisplayString()
	{
		return Name;
	}
}