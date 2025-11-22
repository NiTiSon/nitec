using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class ErrorModuleSymbol : ModuleSymbol, IErrorSymbol
{
	public override string Name { get; }
	public override IEnumerable<IMemberSymbol> Members => [];
	public override LibrarySymbol? Library => null;
	public ImmutableArray<Symbol> Candidates => [];
	public ErrorSymbolReason Reason => ErrorSymbolReason.NotFound;

	public ErrorModuleSymbol(string name)
	{
		Name = name;
	}
}