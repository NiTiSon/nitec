// ReSharper disable FormatStringProblem
namespace NiteCompiler.Diagnostics;

public partial class DiagnosticDescriptor
{
	public static readonly DiagnosticDescriptor
		DuplicateSourceFiles,
		DependenciesInCoreLibrary,
		NotTerminatedMultilineComment,
		NotTerminatedStringLiteral,
		ExpectedToken,
		UnexpectedToken,
		CannotResolveSymbol,
		UnresolvedPredefinedType,
		AmbiguousReference,
		FieldMustHaveEitherTypeClauseOrDefaultValue,
		OnlyTopLevelModuleDeclarationsAreAllowed,
		AccessibilityModifierRequiredBeforeMemberDeclaration,
		IntegralConstantTooLarge,
		IntegralValueCantBeSigned,
		IntegralValueIsGreaterThanMaxValue,
		IntegralValueIsSmallerThanMinValue
		;

	static DiagnosticDescriptor()
	{
		// Compilation
		DuplicateSourceFiles = new("duplicate-source-files",
			"Input files contains duplicates.", DiagnosticSeverity.Warning);
		DependenciesInCoreLibrary = new("dependencies-in-core-lib", "Core library can't have any dependencies.");

		// Lexing
		NotTerminatedMultilineComment = new("not-terminated-multiline-comment",
			"Multi-line comment is not terminated.");
		NotTerminatedStringLiteral = new("not-terminated-string-literal",
			"String literal is not terminated.");

		// Parsing
		UnexpectedToken = new("unexpected-token", "Unexpected token {0}.");
		ExpectedToken = new("expected-token", "Expected token {0}.");
		AccessibilityModifierRequiredBeforeMemberDeclaration = new("accessibility-modifier-required", "Accessibility modifier required before member declaration.");

		// Binding
		CannotResolveSymbol = new("cannot-resolve-symbol", "Cannot resolve symbol.");
		UnresolvedPredefinedType = new("unresolved-predefined-type", "Predefined type `{0}` is not resolved.", DiagnosticSeverity.Warning);
		AmbiguousReference = new("ambiguous-reference", "Ambiguous reference:\n{0}.");
		FieldMustHaveEitherTypeClauseOrDefaultValue = new("field-unresolvable-type", "Field must have either type clause or default value.");
		OnlyTopLevelModuleDeclarationsAreAllowed = new("only-top-level-module-declarations-are-allowed", "Only top-level module declarations are allowed.");

		// Type Checking

		// Value checking
		IntegralConstantTooLarge = new("integral-constant-too-large", "Integral constant too large.");
		IntegralValueCantBeSigned = new("integral-value-cant-be-signed", "Integral value is greater than any signed integral types can be.");
		IntegralValueIsGreaterThanMaxValue = new("integral-value-is-greater-than-max-value",
			"Integral value is greater than max value of specified type.");
		IntegralValueIsSmallerThanMinValue = new("integral-value-is-smaller-than-min-value",
			"Integral value is smaller than min value of specified type.");
	}
}