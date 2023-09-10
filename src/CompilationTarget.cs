namespace NiteCompiler;

public enum CompilationTarget
{
	/// <summary>
	/// .nlib file
	/// </summary>
	Library = 0x1,
	/// <summary>
	/// .exe file
	/// </summary>
	ExecutableImage = 0x2,
	/// <summary>
	/// .nobj file
	/// </summary>
	ObjectFile = 0x4,
}