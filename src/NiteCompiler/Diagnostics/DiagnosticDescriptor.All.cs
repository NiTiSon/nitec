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
		FieldMustHaveEitherTypeClauseOrDefaultValue
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
		FieldMustHaveEitherTypeClauseOrDefaultValue = new("field-unresolvable-type", "Field must have either type clause or default value.");

		// Type Checking
	}
}