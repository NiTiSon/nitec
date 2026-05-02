namespace NiteCompiler.CodeAnalysis.Declarations;

internal enum DeclarationAccessibility : byte
{
	NotDeclaredByError = 0,
	Public = Accessibility.Public,
	Protected = Accessibility.Protected,
	Private = Accessibility.Private,
	Friend = Accessibility.Friend,
	Family = Accessibility.Family,
	Internal = Accessibility.Internal,

	// we want to separate NotDeclaredByError and MissedByInlinedDeclaration,
	// so we do not report[weak-declaration-or-smth] when user forgor to put an accessibility modifier
	MissedByInlinedDeclaration = Internal + 1,
}