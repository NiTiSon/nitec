namespace NiteCompiler.CodeAnalysis.Binding.Pure;

public enum Pureness
{
	/// <summary>
	/// The expression or symbol is not pure.
	/// </summary>
	None = 0,

	/// <summary>
	/// The expression or symbol is pure only with determined target.
	/// </summary>
	TargetSpecific = 1,

	/// <summary>
	/// The expression or symbol is pure.
	/// </summary>
	Full = 2,
}