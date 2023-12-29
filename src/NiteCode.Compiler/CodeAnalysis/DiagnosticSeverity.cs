namespace NiteCode.Compiler.CodeAnalysis;

public enum DiagnosticSeverity
{
	/// <summary>
	/// Information that does not indicate a problem (i.e. not prescriptive).
	/// </summary>
	Information,
	/// <summary>
	/// Something suspicious but allowed.
	/// </summary>
	Warning,
	/// <summary>
	/// SSomething not allowed by the rules of the language or other authority.
	/// </summary>
	Error,
}