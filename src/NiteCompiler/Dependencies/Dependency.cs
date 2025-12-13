namespace NiteCompiler.Dependencies;

public abstract class Dependency
{
	/// <summary>
	/// Determine if dependency contains source code.
	/// </summary>
	public abstract bool ContainSourceCode { get; }
}