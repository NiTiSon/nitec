using System.Collections;
using System.Collections.Generic;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis;
public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly List<Diagnostic> diagnostics;

	public DiagnosticBag()
	{
		diagnostics = new();
	}

	public void ReportWarning(TextAnchor location, string code, string message)
	{
		diagnostics.Add(new (false, location, code, message));
	}
	public void ReportWarning(TextAnchor location, string message)
	{
		diagnostics.Add(new(false, location, message));
	}
	public void ReportError(TextAnchor location, string code, string message)
	{
		diagnostics.Add(new(true, location, code, message));
	}
	public void ReportError(TextAnchor location, string message)
	{
		diagnostics.Add(new(true, location, message));
	}
	public void Report(Diagnostic diagnostic)
	{
		diagnostics.Add(diagnostic);
	}
	public void ReportRange(params Diagnostic[] diagnostic)
	{
		diagnostics.AddRange(diagnostic);
	}
	public void ReportRange(IEnumerable<Diagnostic> diagnostic)
	{
		diagnostics.AddRange(diagnostic);
	}

	public IEnumerator<Diagnostic> GetEnumerator()
	{
		return diagnostics.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return diagnostics.GetEnumerator();
	}
}
