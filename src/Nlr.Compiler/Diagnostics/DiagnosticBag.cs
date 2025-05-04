using System.Collections.Concurrent;
using System.Linq;

namespace Nlr.Compiler.Diagnostics;

public class DiagnosticBag
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
}