using System;

namespace NiteCompiler.CodeAnalysis.Declarations;

[Flags]
internal enum DeclarationModifiers : uint
{
	None = 0,

	Public = 1 << 0,
	Protected = 1 << 1,
	Private = 1 << 2,
	Friend = 1 << 3,
	Family = 1 << 4,
	Internal = 1 << 5,

	Abstract = 1 << 6,
	Override = 1 << 7,
	Sealed = 1 << 8,
	Virtual = 1 << 9,
	Static = 1 << 10,
	Const = 1 << 11,
	Unsafe = 1 << 12,
	Partial = 1 << 13,

	All = (1 << 14) - 1,
	Unset = 1 << 14,

	AccessibilityMask = Public | Protected | Private | Friend | Family | Internal,
}