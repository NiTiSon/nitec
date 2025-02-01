global using static Nlr.Compiler.CodeAnalysis.NiteCode.NiteCodeDiagnostics;
using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public static class NiteCodeDiagnostics
{
	public static DiagnosticDescriptor UnknownSymbol;
	public static DiagnosticDescriptor NoInputFiles;

	static NiteCodeDiagnostics()
	{
		UnknownSymbol = new(
			"unknown-symbol",
			"Unknown symbol are presented",
			Severity.Error
		);
		NoInputFiles = new(
			"no-input-files",
			"No input files are presented",
			Severity.Error
		);
	}
}