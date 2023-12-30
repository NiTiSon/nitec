using NiteCode.Compiler.CodeAnalysis.Text;
using System;

namespace NiteCode.Compiler.CodeAnalysis;

public sealed partial class Diagnostic : IEquatable<Diagnostic>
{
	private readonly object?[] arguments;

	public DiagnosticDescriptor Descriptor { get; }
	public TextLocation Location { get; }

	public string Id
		=> Descriptor.Id;
	public string Message
		=> string.Format(Descriptor.Message, arguments);
	public DiagnosticSeverity Severity
		=> Descriptor.Severity;
	public string? Category
		=> Descriptor.Category;

	private Diagnostic(DiagnosticDescriptor descriptor, TextLocation location, object?[] arguments)
	{
		Descriptor = descriptor;
		Location = location;
		this.arguments = arguments;
	}

	public static Diagnostic Create(DiagnosticDescriptor descriptor, TextLocation location, params object[] arguments)
	{
		return new(descriptor, location, arguments);
	}

	public bool Equals(Diagnostic? other)
		=> ReferenceEquals(this, other);

	public override bool Equals(object? obj)
		=> ReferenceEquals(this, obj);

	public override int GetHashCode()
		=> HashCode.Combine(this);
}