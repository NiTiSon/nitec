using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Translator;

internal sealed class LlvmTranslator
{
	public static void Translate(ImmutableArray<LibrarySymbol> libraries, FunctionSymbol? entryPoint,
		BindingDiagnosticBag diagnostics)
	{
		// Open nlib files -> LibrarySymbol[] as dependencies
		// Parse source files -> LibrarySymbol as current library
	}
}