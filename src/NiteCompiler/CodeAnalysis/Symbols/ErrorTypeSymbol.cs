using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class ErrorTypeSymbol : TypeSymbol
{
	public override string Name { get; }
	public override int Arity { get; }
	public Diagnostic? ErrorInfo { get; }
	public override ContainerSymbol? ContainingSymbol { get; }
	public bool Unreported { get; }
	public ImmutableArray<Symbol> CandidateSymbols { get; }
	public LookupResultKind ResultKind { get; }

	internal ErrorTypeSymbol(NiteCompilation compilation, string name, int arity, Diagnostic? errorInfo, bool unreported)
		: this(compilation.SourceLibrary.GlobalModule, name, arity, errorInfo, unreported, ImmutableArray<Symbol>.Empty, LookupResultKind.Empty)
	{
	}

	internal ErrorTypeSymbol(ContainerSymbol container, string name, int arity, Diagnostic? errorInfo, bool unreported, ImmutableArray<Symbol> candidateSymbols, LookupResultKind resultKind)
	{
		Name = name;
		Arity = arity;
		ContainingSymbol = container;
		ErrorInfo = errorInfo;
		CandidateSymbols = candidateSymbols;
		Unreported = unreported;
		ResultKind = resultKind;
	}

	public override ImmutableArray<Symbol> GetMembers()
	{
		return [];
	}
}