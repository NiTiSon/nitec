using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.Diagnostics;

[DebuggerStepThrough]
public class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly ConcurrentBag<Diagnostic> _diagnostics = [];

	public bool IsEmpty => _diagnostics.IsEmpty;
	public bool HasAnyErrors => _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error);

	public bool HasAnyWarnings => _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Warning);

	public bool HasAnyWarningsOrErrors => _diagnostics.Any(t => t.Severity > DiagnosticSeverity.Warning);

	public void Clear()
	{
		_diagnostics.Clear();
	}

	public void Add(Diagnostic diagnostic)
	{
		_diagnostics.Add(diagnostic);
	}

	private void Add(DiagnosticDescriptor diagnosticDescriptor, ImmutableArray<Location> source, params object?[]? args)
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

	public void AddRange(IEnumerable<Diagnostic> diagnostics)
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
	public void ReportFileDoesNotExists(string path)
	{
		Add(DiagnosticDescriptor.FileDoesNotExist, [], path);
	}

	public void ReportHaveNoPrivilegesToReadFile(string path)
	{
		Add(DiagnosticDescriptor.HaveNoPrivilegesToReadFile, [], path);
	}

	public void ReportUnableToOpenFile(string path)
	{
		Add(DiagnosticDescriptor.UnableToOpenFile, [], path);
	}

	public void ReportDuplicateSourceFiles()
	{
		Add(DiagnosticDescriptor.DuplicateSourceFiles, []);
	}

	public void ReportDependenciesInCoreLibrary()
	{
		Add(DiagnosticDescriptor.DependenciesInCoreLibrary, []);
	}

	public void ReportNotTerminatedMultiLineComment(Location source)
	{
		Add(DiagnosticDescriptor.NotTerminatedMultilineComment, [source]);
	}

	public void ReportNotTerminatedStringLiteral(Location source)
	{
		Add(DiagnosticDescriptor.NotTerminatedStringLiteral, [source]);
	}

	public void ReportExpectedToken(Location source, TokenKind kind)
	{
		Add(DiagnosticDescriptor.ExpectedToken, [source], kind);
	}

	public void ReportUnexpectedToken(Location source, TokenKind currentKind)
	{
		Add(DiagnosticDescriptor.UnexpectedToken, [source], currentKind);
	}

	public void ReportUnresolvedSymbol(Location source)
	{
		Add(DiagnosticDescriptor.CannotResolveSymbol, [source]);
	}

	public void ReportFieldMustHaveEitherTypeClauseOrDefaultValue(Location location)
	{
		Add(DiagnosticDescriptor.FieldMustHaveEitherTypeClauseOrDefaultValue, [location]);
	}

	public void ReportUnresolvedPredefinedType(string type)
	{
		Add(DiagnosticDescriptor.UnresolvedPredefinedType, [], type);
	}

	// public void ReportAmbiguousReference(Location location, params IEnumerable<Symbol> candidates)
	// {
	// 	Add(DiagnosticDescriptor.AmbiguousReference, [location], string.Join(",\n", candidates));
	// }

	public void ReportIntegralConstantIsTooLarge(Location location)
	{
		Add(DiagnosticDescriptor.IntegralConstantTooLarge, [location]);
	}

	public void ReportIntegralValueCantBeSigned(Location location)
	{
		Add(DiagnosticDescriptor.IntegralValueCantBeSigned, [location]);
	}

	public void ReportIntegralValueIsGreaterThanMaxValue(Location location)
	{
		Add(DiagnosticDescriptor.IntegralValueIsGreaterThanMaxValue, [location]);
	}

	public void ReportIntegralValueIsSmallerThanMinValue(Location location)
	{
		Add(DiagnosticDescriptor.IntegralValueIsSmallerThanMinValue, [location]);
	}

	public void ReportOnlyTopLevelModuleDeclarationsAreAllowed(Location location)
	{
		Add(DiagnosticDescriptor.OnlyTopLevelModuleDeclarationsAreAllowed, [location]);
	}

	public void ReportAccessibilityModifierRequiredBeforeMemberDeclaration(Location location)
	{
		Add(DiagnosticDescriptor.AccessibilityModifierRequiredBeforeMemberDeclaration, [location]);
	}

	public void ReportMissingParameterTypeSpecification(Location location)
	{
		Add(DiagnosticDescriptor.MissingParameterTypeSpecification, [location]);
	}
}