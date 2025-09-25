using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class FunctionSymbol : Symbol
{
	public FunctionSymbol(string name, object? parameters, TypeSymbol returnType, Symbol? containingSymbol, FunctionDeclarationSyntax syntax)
	{
		Name = name;
		ReturnType = returnType;
		ContainingSymbol = containingSymbol;
		Syntax = syntax;
	}

	public string Name { get; }
	public TypeSymbol ReturnType { get; }
	public FunctionDeclarationSyntax Syntax { get; }
	public override Symbol? ContainingSymbol { get; }
	public override SymbolKind Kind => SymbolKind.Function;
}