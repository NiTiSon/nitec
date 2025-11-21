using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public class FunctionSymbol : Symbol, IMemberSymbol
{
	public sealed override SymbolKind Kind => SymbolKind.Function;
	public override string Name { get; }
	public IContainerSymbol? ContainingSymbol { get; }
	public bool IsMethod => ContainingSymbol is TypeSymbol;
	public FunctionDeclarationSyntax? Syntax { get; }

	private protected FunctionSymbol(IContainerSymbol containingSymbol, string name, FunctionDeclarationSyntax? syntax = null)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
		Syntax = syntax;
	}
}