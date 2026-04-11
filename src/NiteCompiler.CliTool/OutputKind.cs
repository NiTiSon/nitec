namespace NiteCompiler.CliTool;

public enum OutputKind
{
	/// <summary>
	/// <c>.nlib</c> format with full metadata.
	/// </summary>
	NiTiSLibrary,
	/// <summary>
	/// Native dynamic linking library.
	/// </summary>
	SharedLibrary,
	/// <summary>
	/// Native static library.
	/// </summary>
	StaticLibrary,
	/// <summary>
	/// Executable file.
	/// </summary>
	Executable,
}