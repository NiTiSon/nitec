namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class SymbolVisitor
{
	public virtual void Visit(Symbol? symbol)
	{
		symbol?.Accept(this);
	}

	protected virtual void DefaultVisit(Symbol symbol) { }

	public virtual void VisitModule(ModuleSymbol symbol) => DefaultVisit(symbol);
	public virtual void VisitLibrary(LibrarySymbol symbol) => DefaultVisit(symbol);
	public virtual void VisitFunction(FunctionSymbol symbol) => DefaultVisit(symbol);
}

public abstract class SymbolVisitor<TResult>
{
	public virtual TResult? Visit(Symbol? symbol)
	{
		return symbol != null ? symbol.Accept(this) : default;
	}

	protected virtual TResult? DefaultVisit(Symbol symbol) => default;

	public virtual TResult? VisitModule(ModuleSymbol symbol) => DefaultVisit(symbol);
	public virtual TResult? VisitLibrary(LibrarySymbol symbol) => DefaultVisit(symbol);
	public virtual TResult? VisitFunction(FunctionSymbol symbol) => DefaultVisit(symbol);
}

public abstract class SymbolVisitor<TResult, TArgument>
{
	public virtual TResult? Visit(Symbol? symbol, TArgument arg)
	{
		return symbol != null ? symbol.Accept(this, arg) : default;
	}

	protected virtual TResult? DefaultVisit(Symbol symbol, TArgument arg) => default;

	public virtual TResult? VisitModule(ModuleSymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
	public virtual TResult? VisitLibrary(LibrarySymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
	public virtual TResult? VisitFunction(FunctionSymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
}