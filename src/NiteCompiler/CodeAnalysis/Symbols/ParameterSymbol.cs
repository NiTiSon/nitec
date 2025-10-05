using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ParameterSymbol : Symbol
{
	public override FunctionSymbol ContainingSymbol { get; }
	public override SymbolKind Kind => SymbolKind.Parameter;
	public int Index { get; }
	public FunctionParameterSyntax Syntax { get; }

	public ParameterSymbol(FunctionSymbol containingFunction, int index, FunctionParameterSyntax syntax)
	{
		ContainingSymbol = containingFunction;
		Index = index;
		Syntax = syntax;
	}
}