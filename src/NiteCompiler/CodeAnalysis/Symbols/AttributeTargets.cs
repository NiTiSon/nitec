using System;

namespace NiteCompiler.CodeAnalysis.Symbols;

[Flags]
public enum AttributeTargets
{
	None = 0,
	Function = 1 << 0,
	Type = 1 << 1,
	Field = 1 << 2,
	Constructor = 1 << 3,
	Attribute = 1 << 4,
	All = Function | Type | Field | Constructor | Attribute,
}
