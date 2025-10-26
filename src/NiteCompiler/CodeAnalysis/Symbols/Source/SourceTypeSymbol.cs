using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceTypeSymbol : TypeSymbol, INamedSymbol
{
	public string Name { get; }
	public override IContainerSymbol ContainingSymbol { get; }
	public override List<IMemberSymbol> Members { get; } = [];

	public SourceTypeSymbol(IContainerSymbol containingSymbol, string name)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
	}
}