using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using NiteCompiler.CodeAnalysis.Symbols.Metadata.Nlib;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class GlobalScope
{
	private readonly Dictionary<PredefinedType, PredefinedTypeSymbol> _predefinedSymbols = [];

	public SourceModuleSymbol GlobalModule { get; }
	public DiagnosticBag Diagnostics { get; } = [];
	public ImmutableArray<NlibLibrarySymbol> Dependencies { get; }
	public SourceLibrarySymbol ThisLibrary { get; }
	public IEnumerable<LibrarySymbol> Libraries => ((IEnumerable<LibrarySymbol>)Dependencies).Append(ThisLibrary);

	public GlobalScope(string libraryName, params NlibLibrary[] dependencies)
	{
		ThisLibrary = new(libraryName);
		ThisLibrary.Modules.Add(GlobalModule = new(ThisLibrary, string.Empty));
		Dependencies = [];
	}

	public PredefinedTypeSymbol GetPredefinedType(PredefinedType type)
	{
		ref PredefinedTypeSymbol? symbol = ref CollectionsMarshal.GetValueRefOrAddDefault(_predefinedSymbols, type, out bool exists);

		if (exists)
		{
			return symbol!;
		}

		(string moduleName, string typeName) = PredefinedTypeSymbol.GetNameInfo(type);
		IEnumerable<TypeSymbol> candidates = Libraries
			.SelectMany(t => t.Modules)
			.Where(t => t.Name == moduleName)
			.SelectMany(t => t.Members)
			.OfType<TypeSymbol>()
			.Where(t => t is INamedSymbol named && named.Name == typeName);

		ImmutableArray<TypeSymbol> candidates2 = [..candidates];

		if (candidates2.Length == 0)
		{
			Diagnostics.ReportUnresolvedPredefinedType($"{moduleName}::{typeName}");
			symbol = new PredefinedTypeSymbol(new SourceErrorTypeSymbol(ErrorSymbolReason.NotFound), type);
		}
		else if (candidates2.Length > 1)
		{
			Diagnostics.ReportUnresolvedPredefinedType($"{moduleName}::{typeName}");
			symbol = new PredefinedTypeSymbol(new SourceErrorTypeSymbol(ErrorSymbolReason.Ambiguity, candidates2), type);
		}
		else
		{
			symbol = new PredefinedTypeSymbol(candidates2[0], type);
		}

		return symbol;
	}
}