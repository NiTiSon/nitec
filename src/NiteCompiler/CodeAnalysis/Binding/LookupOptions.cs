using System;

namespace NiteCompiler.CodeAnalysis.Binding;

[Flags]
internal enum LookupOptions
{
	Default = 0,
	ModulesOrTypesOnly = 1 << 1,
	AttributesOnly = 1 << 2,
	MustBeInvocableIfMember = 1 << 3,
	IgnoreFunctionArity = 1 << 4,
}