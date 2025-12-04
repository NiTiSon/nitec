using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Metadata.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	public override string Name { get; }
	public override ImmutableArray<ParameterSymbol> Parameters { get; }

	internal SourceFunctionSymbol(string name, Symbol containingSymbol, ImmutableArray<ParameterSymbol> parameters)
		: base(containingSymbol)
	{
		Name = name;
		Parameters = parameters;
	}
}