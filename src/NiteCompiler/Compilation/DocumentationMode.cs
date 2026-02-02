namespace NiteCompiler.Compilation;

public enum DocumentationMode
{
	/// <summary>
	/// Fully ignores documentation comments.
	/// </summary>
	None = 0,
	/// <summary>
	/// Enables detailed documentation structure parsing.
	/// </summary>
	Parse = 1,
}