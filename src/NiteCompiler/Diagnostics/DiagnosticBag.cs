using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

[DebuggerStepThrough]
public class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly ConcurrentBag<Diagnostic> _diagnostics = [];

	public bool IsEmpty => _diagnostics.IsEmpty;
	public bool HasAnyErrors => _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error);

	public bool HasAnyWarnings => _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Warning);

	public bool HasAnyWarningsOrErrors => _diagnostics.Any(t => t.Severity > DiagnosticSeverity.Warning);

	public void Add(Diagnostic diagnostic)
	{
		_diagnostics.Add(diagnostic);
	}

	private void Add(DiagnosticDescriptor diagnosticDescriptor, params object?[]? args)
	{
		_diagnostics.Add(new Diagnostic(diagnosticDescriptor, null, args));
	}

	private void Add(DiagnosticDescriptor diagnosticDescriptor, SourceSpan source, params object?[]? args)
	{
		_diagnostics.Add(new Diagnostic(diagnosticDescriptor, source, args));
	}

	public void AddRange(ImmutableArray<Diagnostic> diagnostics)
	{
		foreach (var diagnostic in diagnostics)
		{
			Add(diagnostic);
		}
	}

	public void AddRange(ReadOnlySpan<Diagnostic> diagnostics)
	{
		foreach (var diagnostic in diagnostics)
		{
			Add(diagnostic);
		}
	}

	public void DrainInto(DiagnosticBag diagnostics)
	{
		diagnostics.AddRange(_diagnostics.ToArray());
		_diagnostics.Clear();
	}

	public IEnumerator<Diagnostic> GetEnumerator()
	{
		return _diagnostics.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	// REPORTS
	public void ReportDuplicateSourceFiles()
	{
		Add(DiagnosticDescriptor.DuplicateSourceFiles);
	}

	public void ReportNotTerminatedMultiLineComment(SourceSpan source)
	{
		Add(DiagnosticDescriptor.NotTerminatedMultilineComment, source);
	}

	public void ReportNotTerminatedStringLiteral(SourceSpan source)
	{
		Add(DiagnosticDescriptor.NotTerminatedStringLiteral, source);
	}

	public void ReportExpectedToken(SourceSpan source, SyntaxKind kind)
	{
		Add(DiagnosticDescriptor.ExpectedToken, source, kind);
	}

	public void ReportUnexpectedToken(SourceSpan source, SyntaxKind currentKind)
	{
		Add(DiagnosticDescriptor.UnexpectedToken, source, currentKind);
	}

	public void ReportUnresolvedSymbol(SourceSpan source)
	{
		Add(DiagnosticDescriptor.CannotResolveSymbol, source);
	}

	public void ReportFieldMustHaveEitherTypeClauseOrDefaultValue(SourceSpan source)
	{
		Add(DiagnosticDescriptor.FieldMustHaveEitherTypeClauseOrDefaultValue, source);
	}

	public void ReportUnresolvedPredefinedSymbol(string type)
	{
		Add(DiagnosticDescriptor.UnresolvedPredefinedType, type);
	}

	public void ReportIntegralConstantIsTooLarge(SourceSpan source)
	{
		Add(DiagnosticDescriptor.IntegralConstantTooLarge, source);
	}
}