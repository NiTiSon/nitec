using System.Collections;
using System.Collections.Generic;
using Nlr.Compiler.Text;

namespace Nlr.Compiler.Diagnostics;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly List<Diagnostic> diagnostics = new(32);

	public void Report(Diagnostic diagnostic)
	{
		diagnostics.Add(diagnostic);
	}

	public void Report(DiagnosticDescriptor descriptor, SourceText source, TextSpan span)
	{
		diagnostics.Add(new Diagnostic(descriptor, source, span));
	}

	public void Report(DiagnosticDescriptor descriptor)
	{
		diagnostics.Add(new Diagnostic(descriptor));
	}

	public IEnumerator<Diagnostic> GetEnumerator()
	{
		return diagnostics.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)diagnostics).GetEnumerator();
	}
}