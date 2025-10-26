using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceErrorTypeSymbol : TypeSymbol, IErrorTypeSymbol
{
	public override IEnumerable<IMemberSymbol> Members => [];
	public override IContainerSymbol? ContainingSymbol => null;
	public ImmutableArray<Symbol> Candidates { get; }

	public SourceErrorTypeSymbol(params ImmutableArray<Symbol> candidateSymbols)
	{
		Candidates = candidateSymbols;
	}
}