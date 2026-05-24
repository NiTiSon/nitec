namespace NiteCompiler.CodeAnalysis.Binding;

internal enum LookupResultKind : byte
{
	// High values take precedence over lower values.
	Empty,
	NotAModuleNorAType,
	NotAnAttribute,
	WrongArity,
	NotCreatable,
	Inaccessible,
	NotAValue,
	NotInvocable,
	OverloadResolutionFailure,
	Ambiguous,
	MemberGroup,
	Viable,
}