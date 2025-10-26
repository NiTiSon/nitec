using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols.Metadata.Nlib;

public sealed class NlibLibrarySymbol : LibrarySymbol
{
	public override string Name { get; }
	public override List<ModuleSymbol> Modules { get; } // List<NlibModuleSymbol>
}