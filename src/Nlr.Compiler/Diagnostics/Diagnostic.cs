namespace Nlr.Compiler.Diagnostics;

public sealed class Diagnostic
{
	private readonly DiagnosticDescriptor _descriptor;
	private readonly object?[]? _args;

	public DiagnosticSeverity Severity => DefaultSeverity;

	public DiagnosticSeverity DefaultSeverity => _descriptor.DefaultSevevity;

	private Diagnostic(DiagnosticDescriptor descriptor, params object?[]? args)
	{
		_descriptor = descriptor;
		_args = args;
	}
	
	public static Diagnostic Create(
		DiagnosticDescriptor descriptor,
		params object?[]? messageArgs)
	{
		return new(descriptor, messageArgs);
	}
}