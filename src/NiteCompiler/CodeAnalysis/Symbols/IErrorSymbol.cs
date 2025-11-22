using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public interface IErrorSymbol
{
	/// <summary>
	/// Candidates to unresolved symbol (if any).
	/// </summary>
	ImmutableArray<Symbol> Candidates { get; }

	/// <summary>
	/// Error symbol reason.
	/// </summary>
	ErrorSymbolReason Reason { get; }
}