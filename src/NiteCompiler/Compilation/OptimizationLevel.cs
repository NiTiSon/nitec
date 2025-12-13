namespace NiteCompiler.Compilation;

public enum OptimizationLevel
{
	/// <summary>
	/// Just basic optimization.
	/// </summary>
	Default,
	/// <summary>
	/// Turn all optimization off for better debug experience.
	/// </summary>
	Debug,
	/// <summary>
	/// Maximum optimization.
	/// </summary>
	Optimize
}