using System;

namespace NiTiS.Compiler.Diagnostics;

public sealed class Diagnostic
{
	private readonly DiagnosticDescriptor _descriptor;
	private readonly object?[]? _args;

	public DiagnosticSeverity Severity => DefaultSeverity;

	public DiagnosticSeverity DefaultSeverity => _descriptor.DefaultSeverity;

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

	public override string ToString()
	{
		try
		{
			return string.Format(_descriptor.FormatMessage, _args ?? []);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}
}