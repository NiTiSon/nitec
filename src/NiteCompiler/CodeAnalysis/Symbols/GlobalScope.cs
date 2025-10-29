using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols.Metadata.Nlib;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class GlobalScope
{
	public DiagnosticBag Diagnostics { get; } = [];
	public ImmutableArray<NlibLibrarySymbol> Dependencies { get; }
	public SourceLibrarySymbol ThisLibrary { get; }

	public GlobalScope(string libraryName, params NlibLibrary[] dependencies)
	{
		ThisLibrary = new(libraryName);
		Dependencies = [];
	}
}