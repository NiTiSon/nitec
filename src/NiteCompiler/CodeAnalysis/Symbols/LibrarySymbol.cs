namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LibrarySymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Library;
	public override Symbol? ContainingSymbol => null;
	public override LibrarySymbol? ContainingLibrary => null;
	public abstract ModuleSymbol GlobalModule { get; }

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		return Name;
	}

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitLibrary(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLibrary(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitLibrary(this, arg);
	}
}