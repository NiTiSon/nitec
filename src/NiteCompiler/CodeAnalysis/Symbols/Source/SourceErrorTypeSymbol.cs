using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceErrorTypeSymbol : TypeSymbol, IErrorTypeSymbol, ISourceSymbol
{
	public override IEnumerable<IMemberSymbol> Members => [];
	public override IContainerSymbol? ContainingSymbol => null;
	public override TypeSymbol? Parent => null;
	public ImmutableArray<Symbol> Candidates { get; }
	public ErrorSymbolReason Reason { get; }

	public SourceErrorTypeSymbol(ErrorSymbolReason reason, params ImmutableArray<Symbol> candidateSymbols)
	{
		Candidates = candidateSymbols;
		Reason = reason;
	}
}