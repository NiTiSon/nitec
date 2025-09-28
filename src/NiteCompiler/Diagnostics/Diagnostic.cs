using System;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

public sealed class Diagnostic
{
	private readonly DiagnosticDescriptor _descriptor;
	private readonly object?[]? _args;

	public string Id => _descriptor.Id;
	public SourceSpan? Span { get; }
	public DiagnosticSeverity Severity => DefaultSeverity;
	public DiagnosticSeverity DefaultSeverity => _descriptor.DefaultSeverity;

	public string Message
	{
		get
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

	public Diagnostic(DiagnosticDescriptor descriptor, SourceSpan? span, params object?[]? args)
	{
		_descriptor = descriptor;
		Span = span;
		_args = args;
	}

	public override string ToString() => Message;
}