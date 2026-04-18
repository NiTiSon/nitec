namespace NiteCompiler.Compilation;

public enum CompilationStage
{
	/// <summary>
	/// Parsing and Lexing stage.
	/// </summary>
	Parse,

	/// <summary>
	/// Declaration and binding stage.
	/// </summary>
	Declare,

	/// <summary>
	/// Executable code compilation and code generation stage.
	/// </summary>
	Compile,
}

internal static class CompilationStageExtensions
{
	extension(CompilationStage stage)
	{
		public bool IsInclude(CompilationStage other, bool includeEarlierStages)
		{
			return stage == other || stage > other && includeEarlierStages;
		}
	}
}