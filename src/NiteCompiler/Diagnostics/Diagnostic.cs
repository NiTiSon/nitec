using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

public sealed class Diagnostic
{
	private readonly DiagnosticDescriptor _descriptor;
	private readonly object?[]? _args;

	public string Id => _descriptor.Id;
	[Obsolete("Use Locations instead.")]
	public SourceSpan? Span { get; }
	public ImmutableArray<Location> Locations { get; }
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

	[Obsolete("Do not use SourceSpan.")]
	public Diagnostic(DiagnosticDescriptor descriptor, SourceSpan? span, params object?[]? args)
	{
		_descriptor = descriptor;
		Span = span;
		Locations = [];
		_args = args;
	}

	public Diagnostic(DiagnosticDescriptor descriptor, ImmutableArray<Location> locations, params object?[]? args)
	{
		_descriptor = descriptor;
		Locations = locations;
		_args = args;
	}

	public override string ToString() => Message;
}