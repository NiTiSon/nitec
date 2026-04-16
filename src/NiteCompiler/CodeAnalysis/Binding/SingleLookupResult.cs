using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal struct SingleLookupResult
{
	internal readonly LookupResultKind Kind;
	internal readonly Symbol? Symbol;
	internal readonly Diagnostic? Error;

	internal SingleLookupResult(LookupResultKind kind, Symbol? symbol, Diagnostic? error)
	{
		Debug.Assert(symbol is not null || kind == LookupResultKind.Empty);
		Kind = kind;
		Symbol = symbol;
		Error = error;
	}
}