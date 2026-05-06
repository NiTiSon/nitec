using System;
using NiteCompiler.CodeAnalysis.Symbols.Source;

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
	public virtual void VisitType(TypeSymbol symbol) => DefaultVisit(symbol);
	public virtual void VisitLifetimeParameter(LifetimeParameterSymbol parameterSymbol) => DefaultVisit(parameterSymbol);
	public virtual void VisitGenericTypeParameter(GenericTypeParameterSymbol symbol) => DefaultVisit(symbol);
	public virtual void VisitGenericValueParameter(GenericValueParameterSymbol symbol) => DefaultVisit(symbol);
	public virtual void VisitLocalVariable(LocalVariableSymbol symbol) => DefaultVisit(symbol);
	public virtual void VisitParameter(ParameterSymbol symbol) => DefaultVisit(symbol);
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
	public virtual TResult? VisitType(TypeSymbol symbol) => DefaultVisit(symbol);
	public virtual TResult? VisitLifetimeParameter(LifetimeParameterSymbol parameterSymbol) => DefaultVisit(parameterSymbol);
	public virtual TResult? VisitGenericTypeParameter(GenericTypeParameterSymbol symbol) => DefaultVisit(symbol);
	public virtual TResult? VisitGenericValueParameter(GenericValueParameterSymbol symbol) => DefaultVisit(symbol);
	public virtual TResult? VisitLocalVariable(LocalVariableSymbol symbol) => DefaultVisit(symbol);
	public virtual TResult? VisitParameter(ParameterSymbol symbol) => DefaultVisit(symbol);
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
	public virtual TResult? VisitType(TypeSymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
	public virtual TResult? VisitLifetimeParameter(LifetimeParameterSymbol parameterSymbol, TArgument arg) => DefaultVisit(parameterSymbol, arg);
	public virtual TResult? VisitGenericTypeParameter(GenericTypeParameterSymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
	public virtual TResult? VisitGenericValueParameter(GenericValueParameterSymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
	public virtual TResult? VisitLocalVariable(LocalVariableSymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
	public virtual TResult? VisitParameter(ParameterSymbol symbol, TArgument arg) => DefaultVisit(symbol, arg);
}