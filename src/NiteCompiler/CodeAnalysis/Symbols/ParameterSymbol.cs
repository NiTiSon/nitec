using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ParameterSymbol : Symbol
{
	public override FunctionSymbol ContainingSymbol { get; }
	public string Name { get; }
	public override SymbolKind Kind => SymbolKind.Parameter;
	public int Index { get; }
	public FunctionParameterSyntax Syntax { get; }

	public ParameterSymbol(FunctionSymbol containingFunction, string name, int index, FunctionParameterSyntax syntax)
	{
		ContainingSymbol = containingFunction;
		Name = name;
		Index = index;
		Syntax = syntax;
	}
}