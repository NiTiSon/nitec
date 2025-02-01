using Nlr.Compiler.Text;

namespace Nlr.Compiler.Diagnostics;

public sealed class Diagnostic
{
	public DiagnosticDescriptor Descriptor { get; }

	public SourceText? Location { get; }

	public TextSpan Span { get; }

	public Severity Severity => Descriptor.Severity;

	public Diagnostic(DiagnosticDescriptor descriptor, SourceText location, TextSpan span)
	{
		Descriptor = descriptor;
		Location = location;
		Span = span;
	}

	public Diagnostic(DiagnosticDescriptor descriptor)
	{
		Descriptor = descriptor;
		Span = default;
		Location = null;
	}
}