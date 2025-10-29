using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceModuleSymbol : ModuleSymbol, ISourceContainerSymbol
{
	public override string Name { get; }
	public override ICollection<IMemberSymbol> Members { get; } = [];
	public override LibrarySymbol Library { get; }

	public SourceModuleSymbol(SourceLibrarySymbol library, string name)
	{
		Library = library;
		Name = name;
	}
}