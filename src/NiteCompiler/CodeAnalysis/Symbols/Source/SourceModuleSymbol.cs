using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceModuleSymbol : ModuleSymbol
{
	public override string Name { get; }
	public override List<IMemberSymbol> Members { get; } = [];
	public override LibrarySymbol Library { get; }

	public SourceModuleSymbol(SourceLibrarySymbol library, string name)
	{
		Library = library;
		Name = name;
	}
}