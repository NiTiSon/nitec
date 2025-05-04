namespace Nlr.Compiler.Diagnostics;

public sealed class DiagnosticDescriptor
{
	public string Id { get; }
	public string FormatMessage { get; }
	public DiagnosticSeverity DefaultSevevity { get; }

	public DiagnosticDescriptor(string id, string message, DiagnosticSeverity defaultSeverity)
	{
		Id = id;
		FormatMessage = message;
		DefaultSevevity = defaultSeverity;
	}
}