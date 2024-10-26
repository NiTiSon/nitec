namespace Nlr.Compiler.Diagnostics;

public sealed class DiagnosticDescriptor
{
	/// <summary>
	/// Unique identifier of a diagnostic descriptor (e. g. `<i>unused_variable</i>`). 
	/// </summary>
	public string Id { get; }

	/// <summary>
	/// Message describing details about problem.
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// Descriptor default severity.
	/// </summary>
	/// <remarks>
	/// User can increase and lower severity level only if severity is not <see cref="Severity.Error"/>.
	/// </remarks>
	public Severity Severity { get; }

	public DiagnosticDescriptor(string id, string message, Severity severity)
	{
		Id = id;
		Message = message;
		Severity = severity;
	}
}