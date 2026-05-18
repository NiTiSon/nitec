using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class ErrorFieldSymbol : FieldSymbol, IErrorSymbol
{
	public override Symbol? ContainingSymbol { get; }
	public override string Name { get; }
	public override TypeSymbol Type { get; }
	public Diagnostic? ErrorInfo { get; }
	public bool Unreported { get; }
	public ImmutableArray<Symbol> CandidateSymbols { get; }
	public LookupResultKind ResultKind { get; }

	internal ErrorFieldSymbol(NiteCompilation compilation, TypeSymbol? fieldType, string name, Diagnostic? errorInfo, bool unreported)
		: this(compilation.SourceLibrary.GlobalModule, fieldType ?? new ErrorTypeSymbol(compilation, SpecialType.None, "<field_type>", 0, 0, null, false),
			name, errorInfo, unreported, ImmutableArray<Symbol>.Empty, LookupResultKind.Empty)
	{
	}

	internal ErrorFieldSymbol(ContainerSymbol container, TypeSymbol fieldType, string name, Diagnostic? errorInfo, bool unreported,
		ImmutableArray<Symbol> candidateSymbols, LookupResultKind resultKind)
	{
		Name = name;
		Type = fieldType;
		ContainingSymbol = container;
		ErrorInfo = errorInfo;
		CandidateSymbols = candidateSymbols;
		Unreported = unreported;
		ResultKind = resultKind;
	}
}