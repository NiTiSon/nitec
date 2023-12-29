using System;

namespace NiteCode.Compiler.CodeAnalysis;

public sealed class DiagnosticDescriptor : IEquatable<DiagnosticDescriptor>
{
	public string Id { get; }
	public string Title { get; }
	public string Message { get; }

	public string? Category { get; }
	public DiagnosticSeverity Severity { get; }
	public string? HelpUri { get; }

	public DiagnosticDescriptor(string id, string title, string messageFormat, string category, DiagnosticSeverity severity, string? helpUri = null)
	{
		Id = id;
		Title = title;
		Message = messageFormat;
		Category = category;
		Severity = severity;
		HelpUri = helpUri;
	}

	public bool Equals(DiagnosticDescriptor? other)
		=> ReferenceEquals(this, other);

	public override bool Equals(object? obj)
		=> ReferenceEquals(this, obj);

	public override int GetHashCode()
		=> HashCode.Combine(this);
}
