namespace NiteCode.Compiler.CodeAnalysis;

internal static class NiteCodeDiagnosticsDescriptors
{
	public const string Compiler = "Compiler";

	public static DiagnosticDescriptor
		NC0001,
		NC0002,
		NC0003,
		NC0004,
		NC0005;

	static NiteCodeDiagnosticsDescriptors()
	{
		NC0001 = new(nameof(NC0001), "Symbol '{0}' is not allowed", Compiler, DiagnosticSeverity.Error);
		NC0002 = new(nameof(NC0002), "End-of-file found, '*/' expected", Compiler, DiagnosticSeverity.Error);
		NC0003 = new(nameof(NC0003), "Unexpected token '{0}', expected '{1}'", Compiler, DiagnosticSeverity.Error);
		NC0004 = new(nameof(NC0004), "Source file can only contain one module declaration", Compiler, DiagnosticSeverity.Error);
		NC0005 = new(nameof(NC0005), "ModuleDeclaration declaration should be located above all other declarations", Compiler, DiagnosticSeverity.Error);
	}
}