namespace NiteCode.Compiler.CodeAnalysis;

internal static class NiteCodeDiagnosticsDescriptors
{
	public const string Compiler = "Compiler";

	public static DiagnosticDescriptor
		NC0001,
		NC0002,
		NC0003;

	static NiteCodeDiagnosticsDescriptors()
	{
		NC0001 = new(nameof(NC0001), "Invalid symbol", "Symbol '{0}' is not allowed", Compiler, DiagnosticSeverity.Error);
		NC0002 = new(nameof(NC0002), "Unterminated multi-line comment", "End-of-file found, '*/' expected", Compiler, DiagnosticSeverity.Error);
		NC0003 = new(nameof(NC0003), "Unexpected token", "Unexpected token '{0}', expected '{1}'", Compiler, DiagnosticSeverity.Error);
	}
}