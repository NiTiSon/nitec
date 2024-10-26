using Nlr.Compiler.Text;

namespace Nlr.Compiler.Diagnostics;

public sealed class Diagnostic
{
	public DiagnosticDescriptor Descriptor { get; }
	
	public LinePositionSpan Span { get; }

	public Severity Severity => Descriptor.Severity;

	public Diagnostic(DiagnosticDescriptor descriptor, LinePositionSpan span)
	{
		Descriptor = descriptor;
		Span = span;
	}
}