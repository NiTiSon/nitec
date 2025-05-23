using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace Nlr.Compiler.Diagnostics;

[DebuggerStepThrough]
public class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly ConcurrentQueue<Diagnostic> _diagnostics;

	public DiagnosticBag()
	{
		_diagnostics = new();
	}
	
	public bool IsEmpty => _diagnostics.IsEmpty;
	
	public bool HasAnyErrors => _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error);

	public void Add(Diagnostic diagnostic)
	{
		_diagnostics.Enqueue(diagnostic);
	}

	public void Add(DiagnosticDescriptor diagnosticDescriptor, params object?[]? args)
	{
		_diagnostics.Enqueue(Diagnostic.Create(diagnosticDescriptor, args));
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