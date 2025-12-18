
using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceModuleSymbol : ModuleSymbol
{
	private ImmutableArray<Symbol> _members;

	public override string Name { get; }
	public override ImmutableArray<Symbol> Members => _members;
	public override SourceLibrarySymbol ContainingSymbol { get; }
	public ModuleDeclaration? Declaration { get; }
	public override ImmutableArray<Location> Locations { get; }

	public SourceModuleSymbol(SourceLibrarySymbol containingSymbol, ModuleDeclaration declaration)
	{
		Name = declaration.Name;
		ContainingSymbol = containingSymbol;
		Declaration = declaration;
		Locations = [..Declaration.Names.Select(t => t.Location)];
	}

	public SourceModuleSymbol(string name, SourceLibrarySymbol containingSymbol)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
	}

	public void InitializeMembers(ImmutableArray<Symbol> members)
	{
		_members = members;
	}
}