using System.Collections;
using System.Collections.Generic;

namespace Nlr.Compiler.Diagnostics;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly List<Diagnostic> diagnostics;

	public DiagnosticBag()
	{
		diagnostics = new(32);
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