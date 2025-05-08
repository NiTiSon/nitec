using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Nlr.Compiler.Diagnostics;

public sealed class DiagnosticDescriptor
{
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