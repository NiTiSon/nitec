using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLibrarySymbol : LibrarySymbol
{
	private ImmutableArray<SourceModuleSymbol> _symbols;

	public override string Name { get; }
	public override ImmutableArray<ModuleSymbol> Modules => ImmutableArray<ModuleSymbol>.CastUp(_symbols);

	public SourceLibrarySymbol(string name)
	{
		Name = name;
	}

	public void InitializeModules(ImmutableArray<SourceModuleSymbol> symbols)
	{
		_symbols = symbols;
	}
}