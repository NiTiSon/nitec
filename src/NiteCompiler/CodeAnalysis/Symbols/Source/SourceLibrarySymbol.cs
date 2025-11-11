using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLibrarySymbol : LibrarySymbol, ISourceSymbol
{
	public override string Name { get; }
	public override List<SourceModuleSymbol> Modules { get; } = [];

	public SourceLibrarySymbol(string name)
	{
		Name = name;
	}
}