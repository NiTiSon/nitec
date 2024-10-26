namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public sealed class NiteCodeCompilationOptions
{
	public OutputKind OutputKind { get; }

	public NiteCodeCompilationOptions(OutputKind outputKind, OptimizationLevel optimization)
	{
		OutputKind = outputKind;
	}
}
