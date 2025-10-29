using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceTypeSymbol : TypeSymbol, INamedSymbol
{
	private TypeSymbol? _parent;
	public string Name { get; }
	public override IContainerSymbol ContainingSymbol { get; }
	public override TypeSymbol? Parent => _parent;
	public override List<IMemberSymbol> Members { get; } = [];

	public SourceTypeSymbol(IContainerSymbol containingSymbol, string name)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
	}

	public void SetParent(TypeSymbol parent)
	{
		_parent = parent;
	}
}