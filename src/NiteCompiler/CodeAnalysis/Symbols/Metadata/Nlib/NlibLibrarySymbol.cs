using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Metadata.Nlib;

public sealed class NlibLibrarySymbol : LibrarySymbol
{
	public override string Name { get; }
	public override ImmutableArray<ModuleSymbol> Modules { get; }
}