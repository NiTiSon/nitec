namespace NiteCompiler;

public enum Accessibility : byte
{
	/// <summary>
	/// Member always accessible.
	/// </summary>
	Public,
	/// <summary>
	/// Member only accessible within scope and inheritors.
	/// </summary>
	Protected,
	/// <summary>
	/// Member only accessible within scope.
	/// </summary>
	Private,
	/// <summary>
	/// Works like <see cref="Public"/> within same package; otherwise works like <see cref="Protected"/>.
	/// </summary>
	Friend,
	/// <summary>
	/// Works like <see cref="Protected"/> within same package; otherwise is not accessible.
	/// </summary>
	Family,
	/// <summary>
	/// Works like <see cref="Public"/> within same package; otherwise is not accessible.
	/// </summary>
	Internal,
}