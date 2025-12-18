namespace NiteCompiler.Compilation;

public sealed record NiteCompilationOptions
{
	public static readonly NiteCompilationOptions Default = new NiteCompilationOptions();

	public bool IsCoreLibrary { get; set; } = false;
	public OptimizationLevel Optimization { get; set; } = OptimizationLevel.Default;
	public NiteVersion LanguageVersion { get; set; } = NiteVersion.LastStable;
}