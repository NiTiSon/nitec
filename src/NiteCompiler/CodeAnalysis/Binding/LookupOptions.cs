using System;

namespace NiteCompiler.CodeAnalysis.Binding;

[Flags]
internal enum LookupOptions
{
	Default = 0,
	ModulesOrTypesOnly = 1 << 1,
}