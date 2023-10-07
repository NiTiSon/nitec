using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis;
public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly List<Diagnostic> diagnostics;

	public DiagnosticBag()
	{
		diagnostics = new();
	}

	public bool ContainsError
		=> diagnostics.Any(t => t.IsError);

	public void ReportWarning(CodeSource source, TextAnchor location, string code, string message)
	{
		diagnostics.Add(new (false, source, location, code, message));
	}
	public void ReportWarning(CodeSource source, TextAnchor location, string message)
	{
		diagnostics.Add(new(false, source, location, message));
	}
	public void ReportError(CodeSource source, TextAnchor location, string code, string message)
	{
		diagnostics.Add(new(true, source, location, code, message));
	}
	public void ReportError(CodeSource source, TextAnchor location, string message)
	{
		diagnostics.Add(new(true, source, location, message));
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
