using System;
using System.Threading;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Compilation;

public sealed record NiteCompilationOptions
{
	public static readonly NiteCompilationOptions Default = new NiteCompilationOptions();

	public bool IsCoreLibrary { get; set; } = false;
	public OptimizationLevel Optimization { get; set; } = OptimizationLevel.Default;
	public DocumentationMode DocumentationMode { get; set; } = DocumentationMode.Parse;
	public NiteVersion LanguageVersion { get; set; } = NiteVersion.LastStable;
	public DateTime CompilationTime { get; set; } = DateTime.Now;
	public bool ConcurrentBuild { get; set; } = true;
	public bool DeterministicBuild { get; set; }

	public Diagnostic? FilterDiagnostic(Diagnostic diagnostic, CancellationToken cancellationToken = default)
	{
		// TODO: Implement -wae-, -wae+, etc.
		return diagnostic;
	}
}