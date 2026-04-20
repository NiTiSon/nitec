// ReSharper disable FormatStringProblem
namespace NiteCompiler.Diagnostics;

public partial class DiagnosticDescriptor
{
	public static readonly DiagnosticDescriptor
		FileDoesNotExist,
		HaveNoPrivilegesToReadFile,
		UnableToOpenFile,
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
		MissingParameterTypeSpecification,
		MustReturnValue,
		CannotReturnValue,
		WrongReturnExpressionType,
		CannotImplicitlyConvert,
		IntegralConstantTooLarge,
		IntegralValueCantBeSigned,
		IntegralValueIsGreaterThanMaxValue,
		IntegralValueIsSmallerThanMinValue,
		InternalError
		;

	static DiagnosticDescriptor()
	{
		// Compilation
		FileDoesNotExist = new("file-not-found", "File '{0}' does not exist.");
		HaveNoPrivilegesToReadFile = new("have-no-privileges-to-read-file", "Process have not enough privileges to read file '{0}'.");
		UnableToOpenFile = new("unable-to-open-file", "Unable to open the file '{0}'.");
		DuplicateSourceFiles = new("duplicate-source-files", "Input files contains duplicates.");
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
		MissingParameterTypeSpecification = new("missing-parameter-type", "Type specification required for parameter syntax.");

		// Binding
		CannotResolveSymbol = new("cannot-resolve-symbol", "Cannot resolve symbol.");
		UnresolvedPredefinedType = new("unresolved-predefined-type", "Predefined type `{0}` is not resolved.");
		AmbiguousReference = new("ambiguous-reference", "Ambiguous reference:\n{0}.");
		FieldMustHaveEitherTypeClauseOrDefaultValue = new("field-unresolvable-type", "Field must have either type clause or default value.");
		OnlyTopLevelModuleDeclarationsAreAllowed = new("only-top-level-module-declarations-are-allowed", "Only top-level module declarations are allowed.");

		// Type Checking
		MustReturnValue = new("must-return", "Return statement must return a value.");
		CannotReturnValue = new("cannot-return", "Return statement cannot return a value.");
		WrongReturnExpressionType = new("wrong-return-expression",
			"Return statement expression type is not convertable to the function return type.");
		CannotImplicitlyConvert = new("cannot-implicitly-convert", "Cannot implicitly cast type '{0}' to the '{1}'.");

		// Value checking
		IntegralConstantTooLarge = new("integral-constant-too-large", "Integral constant too large.");
		IntegralValueCantBeSigned = new("integral-value-cant-be-signed", "Integral value is greater than any signed integral types can be.");
		IntegralValueIsGreaterThanMaxValue = new("integral-value-is-greater-than-max-value",
			"Integral value is greater than max value of specified type.");
		IntegralValueIsSmallerThanMinValue = new("integral-value-is-smaller-than-min-value",
			"Integral value is smaller than min value of specified type.");

		InternalError = new("internal-error", "Critical internal compiler error. Please report this to the nitec developer.\n{0}.");
	}
}