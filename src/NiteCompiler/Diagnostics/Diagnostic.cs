using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

public sealed class Diagnostic
{
	private readonly DiagnosticDescriptor _descriptor;
	public bool IsSuppressed { get; }
	private readonly object?[]? _args;

	public string Id => _descriptor.Id;
	public ImmutableArray<Location> Locations { get; }
	public DiagnosticSeverity Severity => DefaultSeverity;
	public DiagnosticSeverity DefaultSeverity => _descriptor.DefaultSeverity;
	public bool IsUnsuppressableError => _descriptor.DefaultSeverity == DiagnosticSeverity.Error;

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

	public Diagnostic(DiagnosticDescriptor descriptor, ImmutableArray<Location> locations, params object?[]? args)
	{
		_descriptor = descriptor;
		Locations = locations;
		_args = args;
	}

	public Diagnostic(DiagnosticDescriptor descriptor, bool isSuppressed, ImmutableArray<Location> locations, params object?[]? args)
	{
		_descriptor = descriptor;
		IsSuppressed = isSuppressed;
		Locations = locations;
		_args = args;
	}

	public override string ToString() => Message;
}