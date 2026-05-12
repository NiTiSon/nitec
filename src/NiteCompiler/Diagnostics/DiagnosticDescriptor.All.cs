// ReSharper disable FormatStringProblem
namespace NiteCompiler.Diagnostics;

public partial class DiagnosticDescriptor
{
	public static readonly DiagnosticDescriptor
		FileDoesNotExist,
		HaveNoPrivilegesToReadFile,
		UnableToOpenFile,
		UnableToWriteFile,
		DuplicateSourceFiles,
		DependenciesInCoreLibrary,
		UnterminatedMultilineComment,
		UnterminatedStringLiteral,
		UnterminatedEscapedIdentifier,
		InvalidCharacterLiteral,
		InvalidCharacterLiteralEncoding,
		ExpectedToken,
		UnexpectedToken,
		EmptyLifetimeName,
		DuplicateLifetimeParameter,
		DuplicateLifetimeConstraintClause,
		GenericsIsNotApplicableOnModuleName,
		GenericsIsNotApplicableOnGenericTypeName,
		CannotResolveSymbol,
		UnresolvedPredefinedType,
		AmbiguousReference,
		DanglingReference,
		ReferenceEscapesScope,
		FieldMustHaveEitherTypeClauseOrDefaultValue,
		OnlyTopLevelModuleDeclarationsAreAllowed,
		ImplicitlyTypedVariableMustBeInitialized,
		AccessibilityModifierRequiredBeforeMemberDeclaration,
		MissingParameterTypeSpecification,
		MustReturnValue,
		CannotReturnValue,
		WrongReturnExpressionType,
		MissingReturnStatement,
		CannotImplicitlyConvert,
		CannotUseAsLValue,
		CannotUseAsRValue,
		CannotDereferenceNonReference,
		IntegralConstantTooLarge,
		IntegralValueCantBeSigned,
		IntegralValueIsGreaterThanMaxValue,
		IntegralValueIsSmallerThanMinValue,
		InternalError,
		LinkerNotZeroReturnCode
		;

	static DiagnosticDescriptor()
	{
		// Compilation
		FileDoesNotExist = new("file-not-found", "File '{0}' does not exist.");
		HaveNoPrivilegesToReadFile = new("have-no-privileges-to-read-file", "Process have not enough privileges to read file '{0}'.");
		UnableToOpenFile = new("unable-to-open-file", "Unable to open the file '{0}'.");
		UnableToWriteFile = new("unable-to-write-file", "Unable to write a file '{0}'.");
		DuplicateSourceFiles = new("duplicate-source-files", "Input files contains duplicates.");
		DependenciesInCoreLibrary = new("dependencies-in-core-lib", "Core library can't have any dependencies.");

		// Lexing
		UnterminatedMultilineComment = new("unterminated-multiline-comment",
			"Multi-line comment is not terminated.");
		UnterminatedStringLiteral = new("unterminated-string-literal",
			"String literal is not terminated.");
		UnterminatedEscapedIdentifier = new("unterminated-escape-identifier",
			"Escaped identifier is not terminated.");
		InvalidCharacterLiteral = new("invalid-character-literal",
			"Character literal must contain exactly one Unicode scalar value.");
		InvalidCharacterLiteralEncoding = new("invalid-character-literal-encoding",
			"Character literal cannot be represented as {0}: it requires {1} byte(s).");

		// Parsing
		UnexpectedToken = new("unexpected-token", "Unexpected token {0}.");
		GenericsIsNotApplicableOnModuleName = new("generics-not-applicable-on-module-name", "Module name cannot contain any generic parameters.");
		GenericsIsNotApplicableOnGenericTypeName = new("generics-not-applicable-on-generic-type-name", "Generic type name cannot contain any generic parameters.");
		ExpectedToken = new("expected-token", "Expected token {0}.");
		EmptyLifetimeName = new("empty-lifetime-name", "The lifetime identifier cannot be empty.");
		DuplicateLifetimeParameter = new("lifetime-parameter-duplicate", "Lifetime parameter with the same name already exists.");
		DuplicateLifetimeConstraintClause = new("duplicate-lifetime-constraint-clause", "Constraint for {0} lifetime is already declared.");
		AccessibilityModifierRequiredBeforeMemberDeclaration = new("accessibility-modifier-required", "Accessibility modifier required before member declaration.");
		MissingParameterTypeSpecification = new("missing-parameter-type", "Type specification required for parameter syntax.");

		// Binding
		CannotResolveSymbol = new("cannot-resolve-symbol", "Cannot resolve symbol.");
		UnresolvedPredefinedType = new("unresolved-predefined-type", "Predefined type `{0}` is not resolved.");
		AmbiguousReference = new("ambiguous-reference", "Ambiguous reference:\n{0}.");
		DanglingReference = new("dangling-reference", "Reference to '{0}' may outlive the storage of '{1}'.");
		ReferenceEscapesScope = new("reference-escapes-scope", "Reference to local variable '{0}' escapes its scope.");
		FieldMustHaveEitherTypeClauseOrDefaultValue = new("field-unresolvable-type", "Field must have either type clause or default value.");
		OnlyTopLevelModuleDeclarationsAreAllowed = new("only-top-level-module-declarations-are-allowed", "Only top-level module declarations are allowed.");
		ImplicitlyTypedVariableMustBeInitialized = new("variable-must-be-initialized", "Implicitly typed variable must be initialized.");

		// Type Checking
		MustReturnValue = new("must-return", "Return statement must return a value.");
		CannotReturnValue = new("cannot-return", "Return statement cannot return a value.");
		WrongReturnExpressionType = new("wrong-return-expression",
			"Return statement expression type '{0}' is not implicitly convertable to the function return type '{1}'.");
		MissingReturnStatement = new("missing-return", "Return statement is required.");
		CannotImplicitlyConvert = new("cannot-implicitly-convert", "Cannot implicitly cast type '{0}' to the '{1}'.");
		CannotUseAsLValue = new("cannot-use-as-lvalue", "Expression cannot be used as an assignment target or referenced storage location.");
		CannotUseAsRValue = new("cannot-use-as-rvalue", "Expression cannot be used as a value.");
		CannotDereferenceNonReference = new("cannot-dereference-non-reference", "Cannot dereference non-reference type '{0}'.");

		// Value checking
		IntegralConstantTooLarge = new("integral-constant-too-large", "Integral constant too large.");
		IntegralValueCantBeSigned = new("integral-value-cant-be-signed", "Integral value is greater than any signed integral types can be.");
		IntegralValueIsGreaterThanMaxValue = new("integral-value-is-greater-than-max-value",
			"Integral value is greater than max value of specified type.");
		IntegralValueIsSmallerThanMinValue = new("integral-value-is-smaller-than-min-value",
			"Integral value is smaller than min value of specified type.");

		InternalError = new("internal-error", "Critical internal compiler error. Please report this to the nitec developer.\n{0}.");
		LinkerNotZeroReturnCode = new("linker-not-zero-exit-code", "{0}");
	}
}
