using System;
using System.IO;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;

namespace NiteCompiler.Metadata;

internal sealed class MetadataLibraryBuilder : SymbolVisitor
{
	public static object Translate(SourceLibrarySymbol sourceLibrary, Stream library)
	{
		Guard.CanWrite(library);
		// Library:
		// + name
		// + dependencies
		throw new NotImplementedException();
	}
}