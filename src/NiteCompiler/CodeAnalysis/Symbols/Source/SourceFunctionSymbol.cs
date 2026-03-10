using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	public override Symbol ContainingSymbol { get; }
	public FunctionDeclarationSyntax Syntax { get; }

	public SourceFunctionSymbol(Symbol containingSymbol, FunctionDeclarationSyntax syntax)
	{
		Debug.Assert(containingSymbol != null);

		ContainingSymbol = containingSymbol;
		Syntax = syntax;
	}

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitFunction(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitFunction(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitFunction(this, arg);
	}

	public Binder? TryGetInFunctionBinder()
	{
		return null;
	}

	public ExecutableCodeBinder? TryGetBodyBinder()
	{
		Binder? inFunctionBinder = TryGetInFunctionBinder();
		return inFunctionBinder == null ? null : new ExecutableCodeBinder(Syntax, this, inFunctionBinder);
	}
}