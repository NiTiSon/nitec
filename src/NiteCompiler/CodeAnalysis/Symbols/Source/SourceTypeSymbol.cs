using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceTypeSymbol : TypeSymbol
{
	private SpecialType _specialType;
	private TypeSymbol? _parentType;
	private ImmutableArray<Symbol> _members;
	public override string Name { get; }
	public override Symbol ContainingSymbol { get; }
	public TypeDeclaration Declaration { get; }
	public override ImmutableArray<Symbol> Members => _members;
	public override TypeSymbol? Parent => _parentType;
	public override SpecialType SpecialType => _specialType;

	public override ImmutableArray<Location> Locations => [..Declaration.Names.Select(t => t.Location)];

	public SourceTypeSymbol(Symbol containingSymbol, TypeDeclaration declaration)
	{
		Name = declaration.Name;
		ContainingSymbol = containingSymbol;
		Declaration = declaration;
	}

	public void SetSpecialType(SpecialType specialType)
	{
		_specialType = specialType;
	}

	public void AddMember(Symbol member)
	{
		_members = _members.Add(member);
	}

	public void SetParentType(TypeSymbol parentType)
	{
		_parentType = parentType;
	}
}