using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class FunctionSymbol : Symbol
{
	public FunctionSymbol(string name, Symbol containingSymbol, FunctionDeclarationSyntax syntax)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
		Syntax = syntax;
	}

	public override string Name { get; }
	public TypeSymbol ReturnType { get; internal set; } = null!;

	/// <summary>
	/// Base function that overriden by this function.
	/// </summary>
	public FunctionSymbol? OverridenMethod { get; }

	public bool IsMethod => ContainingSymbol is TypeSymbol;
	public FunctionDeclarationSyntax Syntax { get; }
	public override Symbol ContainingSymbol { get; }

	public TypeSymbol? ContainingType
	{
		get
		{
			Symbol? symbol = ContainingSymbol;

			while (symbol != null || symbol is TypeSymbol)
			{
				symbol = symbol.ContainingSymbol;
			}

			return symbol as TypeSymbol;
		}
	}

	public ImmutableArray<ParameterSymbol> Parameters { get; internal set; }

	public override SymbolKind Kind => SymbolKind.Function;
}