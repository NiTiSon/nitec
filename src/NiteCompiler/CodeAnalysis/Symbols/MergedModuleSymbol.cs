using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class MergedModuleSymbol : ModuleSymbol
{
	public override string Name { get; }
	public ImmutableArray<ModuleSymbol> Modules { get; }
	public override LibrarySymbol? Library => null;

	public MergedModuleSymbol(string name, params ImmutableArray<ModuleSymbol> modules)
	{
		Name = name;
		Modules = modules;
	}

	public override IEnumerable<IMemberSymbol> Members
	{
		get
		{
			return Modules.SelectMany(t => t.Members);
		}
	}
}