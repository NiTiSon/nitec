using System.Collections.Immutable;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceModuleSymbol : ModuleSymbol
{
	private readonly FreezableArray<Location> _locations = new();
	private readonly FreezableArray<Symbol> _members = new();

	public override string Name { get; }
	public override ImmutableArray<Symbol> Members => _members;
	public override LibrarySymbol ContainingSymbol { get; }
	public override ImmutableArray<Location> Locations => _locations;

	public SourceModuleSymbol(string name, LibrarySymbol containingLibrary)
	{
		Name = name;
		ContainingSymbol = containingLibrary;
	}

	internal void AddDefinition(Location location)
	{
		_locations.Add(location);
	}

	internal void AddMember(Symbol member)
	{
		_members.Add(member);
	}
}