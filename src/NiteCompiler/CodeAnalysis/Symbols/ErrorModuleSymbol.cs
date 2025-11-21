using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class ErrorModuleSymbol : ModuleSymbol, IErrorSymbol
{
	public override string Name { get; }
	public override IEnumerable<IMemberSymbol> Members => [];
	public override LibrarySymbol? Library => null;
	public ErrorSymbolReason Reason => ErrorSymbolReason.NotFound;

	public ErrorModuleSymbol(string name)
	{
		Name = name;
	}

}