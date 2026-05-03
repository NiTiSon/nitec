namespace NiteCompiler.CodeAnalysis.Symbols;

public enum TypeKind : byte
{
	/// <summary>
	/// Type is unknown.
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// Type is a simple data type.
	/// </summary>
	SimpleType = 1,

	/// <summary>
	/// Type is an interface.
	/// </summary>
	Interface = 2,

	/// <summary>
	/// Type is an error type.
	/// </summary>
	ErrorType = 3,

	/// <summary>
	/// Type is a pointer type.
	/// </summary>
	Pointer = 4,

	/// <summary>
	/// Type is a reference type.
	/// </summary>
	Reference = 5,
}