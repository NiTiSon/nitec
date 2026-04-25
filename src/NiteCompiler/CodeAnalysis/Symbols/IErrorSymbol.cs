using System.Collections.Immutable;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public interface IErrorSymbol
{
	public Diagnostic? ErrorInfo { get; }
	public bool Unreported { get; }
	public ImmutableArray<Symbol> CandidateSymbols { get; }
}