// ReSharper disable FormatStringProblem
namespace NiteCompiler.Diagnostics;

public partial class DiagnosticDescriptor
{
	public static DiagnosticDescriptor
		DuplicateSourceFiles,
		NotTerminatedMultilineComment,
		NotTerminatedStringLiteral,
		ExpectedToken,
		CannotResolveSymbol
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
		ExpectedToken = new("expected-token", "Expected token {0}.");

		// Binding
		CannotResolveSymbol = new("cannot-resolve-symbol", "Cannot resolve symbol.");

		// Type Checking
	}
}