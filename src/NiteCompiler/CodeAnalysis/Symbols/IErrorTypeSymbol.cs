using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public interface IErrorTypeSymbol
{
	/// <summary>
	/// Either empty or 2+.
	/// </summary>
	ImmutableArray<Symbol> Candidates { get; }

	ErrorSymbolReason Reason { get; }
}