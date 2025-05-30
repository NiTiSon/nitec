namespace Nlr.Compiler.Symbols;

/// <summary>
/// Defines the accessibility levels for symbols in the code.
/// </summary>
public enum Accessibility
{
	/// <summary>
	/// Accessible only within the container.
	/// </summary>
	Private = 0,

	/// <summary>
	/// Threat as <see cref="Protected"/> within same package, otherwise as <see cref="Private"/>.
	/// </summary>
	Family = 1,

	/// <summary>
	/// Threat as <see cref="Public"/> within same package, otherwise as <see cref="Private"/>.
	/// </summary>
	Internal = 2,

	/// <summary>
	/// Accessible from the container and all derived types.
	/// </summary>
	Protected = 3,

	/// <summary>
	/// Threat as <see cref="Public"/> within same package, otherwise as <see cref="Protected"/>.
	/// </summary>
	Friend = 4,

	/// <summary>
	/// Always accessible.
	/// </summary>
	Public = 5,
}