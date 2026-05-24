namespace NiteCompiler.CodeAnalysis.Binding;

internal enum LookupResultKind : byte
{
	// High values take precedence over lower values.
	Empty,
	Viable,
}