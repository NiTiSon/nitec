using System;

namespace NiteCompiler.CodeAnalysis.Declarations;

[Flags]
internal enum DeclarationModifiers
{
	None = 0,
	Partial = 1 << 0,
	Abstract = 1 << 1,
	Virtual = 1 << 2,
	Sealed = 1 << 3,
	Unsized = 1 << 4,

	Unset = 1 << 5,
	All = (Unset) - 1,
}