using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public interface IErrorTypeSymbol : IErrorSymbol
{
	/// <summary>
	/// Either empty or 2+.
	/// </summary>
	ImmutableArray<Symbol> Candidates { get; }
}

public interface IErrorSymbol
{
	/// <summary>
	/// Error symbol reason.
	/// </summary>
	ErrorSymbolReason Reason { get; }
}