using System.Diagnostics.CodeAnalysis;

namespace NiteCompiler.Diagnostics;

public sealed partial class DiagnosticDescriptor
{
	/// <summary>
	/// Unique identifier for each error type.
	/// </summary>
	[StringSyntax("a-zA-Z0-9_")]
	public string Id { get; }
	public string FormatMessage { get; }
	public DiagnosticSeverity DefaultSeverity { get; }

	public DiagnosticDescriptor(
		string id,
		[StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
		DiagnosticSeverity defaultSeverity = DiagnosticSeverity.Error)
	{
		Id = id;
		FormatMessage = message;
		DefaultSeverity = defaultSeverity;
	}
}