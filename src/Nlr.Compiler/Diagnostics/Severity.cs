namespace Nlr.Compiler.Diagnostics;

public enum Severity
{
	/// <summary>
	/// Something not allowed by compiler.
	/// </summary>
	/// <remarks>
	/// Compilation with at least one error will not compile.
	/// </remarks>
	Error,
	/// <summary>
	/// Something suspicious but allowed.
	/// </summary>
	Warning,
	/// <summary>
	/// Information that does not indicate a problem (i.e. not prescriptive).
	/// </summary>
	Suggestion,
}
