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

	public string Name { get; }
	public TypeSymbol ReturnType { get; internal set; } = null!;

	/// <summary>
	/// Base function that overriden by this function.
	/// </summary>
	public FunctionSymbol? OverridenMethod { get; }

	public bool IsMethod => ContainingSymbol is TypeSymbol;
	public FunctionDeclarationSyntax Syntax { get; }
	public override Symbol? ContainingSymbol { get; }
	public override SymbolKind Kind => SymbolKind.Function;
}