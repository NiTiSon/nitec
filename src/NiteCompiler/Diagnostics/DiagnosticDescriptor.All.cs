// ReSharper disable FormatStringProblem
namespace NiteCompiler.Diagnostics;

public partial class DiagnosticDescriptor
{
	public static readonly DiagnosticDescriptor
		DuplicateSourceFiles,
		NotTerminatedMultilineComment,
		NotTerminatedStringLiteral,
		ExpectedToken,
		UnexpectedToken,
		CannotResolveSymbol,
		UnresolvedPredefinedType,
		FieldMustHaveEitherTypeClauseOrDefaultValue,
		IntegralConstantTooLarge
		;

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

		// Parsing
		UnexpectedToken = new("unexpected-token", "Unexpected token {0}.");
		ExpectedToken = new("expected-token", "Expected token {0}.");

		// Binding
		CannotResolveSymbol = new("cannot-resolve-symbol", "Cannot resolve symbol.");
		UnresolvedPredefinedType = new("unresolved-predefined-type", "Predefined type {0} is not resolved.", DiagnosticSeverity.Warning);
		FieldMustHaveEitherTypeClauseOrDefaultValue = new("field-unresolvable-type", "Field must have either type clause or default value.");

		// Type Checking

		// Value checking
		IntegralConstantTooLarge = new("integral-constant-too-large", "Integral constant too large.");
	}
}