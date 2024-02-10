namespace NiteCode.Compiler.CodeAnalysis;

internal static class NiteCodeDiagnosticsDescriptors
{
	public const string Compiler = "Compiler";

	public static DiagnosticDescriptor
		NC0001, // Symbol not allowed
		NC0002, // Multi-line comment not closed
		NC0003, // Unexpected token
		NC0004, // More than one module declaration
		NC0005, // Module declaration wrong location
		NC0006; // Zero byte is founded inside text

	static NiteCodeDiagnosticsDescriptors()
	{
		NC0001 = new(nameof(NC0001), "Symbol '{0}' is not allowed", Compiler, DiagnosticSeverity.Error);
		NC0002 = new(nameof(NC0002), "End-of-file found, '*/' expected", Compiler, DiagnosticSeverity.Error);
		NC0003 = new(nameof(NC0003), "Unexpected token '{0}', expected '{1}'", Compiler, DiagnosticSeverity.Error);
		NC0004 = new(nameof(NC0004), "Source file can only contain one module declaration", Compiler, DiagnosticSeverity.Error);
		NC0005 = new(nameof(NC0005), "ModuleDeclaration declaration should be located above all other declarations", Compiler, DiagnosticSeverity.Error);
		NC0006 = new(nameof(NC0006), "Zero byte founded inside text.", Compiler, DiagnosticSeverity.Error);
	}
}