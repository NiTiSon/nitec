namespace NiteCompiler.Compilation;

public sealed record NiteCompilationOptions
{
	public static readonly NiteCompilationOptions Default = new NiteCompilationOptions();

	public bool IsCoreLibrary { get; set; } = false;
	public OptimizationLevel Optimization { get; set; } = OptimizationLevel.Default;
	public DocumentationMode DocumentationMode { get; set; } = DocumentationMode.Parse;
	public NiteVersion LanguageVersion { get; set; } = NiteVersion.LastStable;
}