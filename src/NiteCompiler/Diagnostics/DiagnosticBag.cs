using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NiteCompiler.Diagnostics;

[DebuggerStepThrough]
public class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly ConcurrentBag<Diagnostic> _diagnostics = [];

	public bool IsEmpty => _diagnostics.IsEmpty;
	public bool HasAnyErrors => _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error);

	public bool HasAnyWarnings => _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Warning);

	public void Add(Diagnostic diagnostic)
	{
		_diagnostics.Add(diagnostic);
	}

	public void Add(DiagnosticDescriptor diagnosticDescriptor, params object?[]? args)
	{
		_diagnostics.Add(Diagnostic.Create(diagnosticDescriptor, args));
	}

	public IEnumerator<Diagnostic> GetEnumerator()
	{
		return _diagnostics.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}