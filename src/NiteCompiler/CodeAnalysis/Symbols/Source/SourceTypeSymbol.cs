using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceTypeSymbol : TypeSymbol, INamedSymbol, ISourceContainerSymbol
{
	private TypeSymbol? _parent;
	public string Name { get; }
	public TypeDeclarationSyntax? Syntax { get; }
	public override IContainerSymbol ContainingSymbol { get; }
	public override TypeSymbol? Parent => _parent;
	public override ICollection<IMemberSymbol> Members { get; } = [];

	public SourceTypeSymbol(IContainerSymbol containingSymbol, string name, TypeDeclarationSyntax? syntax = null)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
		Syntax = syntax;
	}

	public void SetParent(TypeSymbol parent)
	{
		_parent = parent;
	}
}