using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public interface IErrorType
{
	public Diagnostic? ErrorInfo { get; }
	public bool Unreported { get; }
	public ImmutableArray<Symbol> CandidateSymbols { get; }
}