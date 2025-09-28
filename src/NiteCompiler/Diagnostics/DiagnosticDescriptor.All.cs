namespace NiteCompiler.Diagnostics;

public partial class DiagnosticDescriptor
{
	public static DiagnosticDescriptor
		DuplicateSourceFiles,
		NotTerminatedMultilineComment,
		NotTerminatedStringLiteral,
		InvalidCharacter,
		InvalidEscapeSequence,
		UnexpectedToken,
		ExpectedToken,
		InvalidExpression,
		InvalidTypeSyntax,
		UndefinedName,
		VariableAlreadyDeclared,
		CannotAssignToReadonly,
		UndefinedType,
		UndefinedFunction,
		FunctionOverloadResolutionFailed,
		CannotConvertType,
		UndefinedUnaryOperator,
		UndefinedBinaryOperator,
		ReturnTypeMismatch;

	static DiagnosticDescriptor()
	{
		// Compilation
		DuplicateSourceFiles = new("duplicate-source-files",
			"Input files contains duplicates.", DiagnosticSeverity.Warning);

		// Lexing
		NotTerminatedMultilineComment = new("not-terminated-multiline-comment",
			"Multi-line comment is not terminated.");
		NotTerminatedStringLiteral = new("not-terminated-string-literal",
			"String literal is not terminated.");
		InvalidCharacter = new("invalid-character",
			"Invalid character in source.");
		InvalidEscapeSequence = new("invalid-escape-sequence",
			"Invalid escape sequence in string or char literal.");

		// Parsing
		UnexpectedToken = new("unexpected-token", "Unexpected token '{0}', expected {1}.");
		ExpectedToken = new("expected-token", "Expected token {0}.");
		InvalidExpression = new("invalid-expression", "Invalid expression syntax.");
		InvalidTypeSyntax = new("invalid-type-syntax", "Invalid type syntax.");

		// Binding
		UndefinedName = new("undefined-name",
			"The name '{0}' does not exist in the current context.");
		VariableAlreadyDeclared = new("variable-already-declared",
				"The variable '{0}' is already declared in this scope.");
		CannotAssignToReadonly = new("cannot-assign-readonly",
			"Cannot assign to readonly variable '{0}'.");
		UndefinedType = new("undefined-type",
			"The type '{0}' could not be found.");
		UndefinedFunction = new("undefined-function",
			"The function '{0}' does not exist.");
		FunctionOverloadResolutionFailed = new("overload-resolution-failed",
			"No overload for function '{0}' matches the given arguments.");

		// Type Checking
		CannotConvertType = new("cannot-convert-type",
			"Cannot convert from type '{0}' to type '{1}'.");
		UndefinedUnaryOperator = new("undefined-unary-operator",
			"Unary operator '{0}' is not defined for type '{1}'.");
		UndefinedBinaryOperator = new("undefined-binary-operator",
			"Binary operator '{0}' is not defined for types '{1}' and '{2}'.");
		ReturnTypeMismatch = new("return-type-mismatch",
			"Cannot return value of type '{0}' from function with return type '{1}'.");
	}
}