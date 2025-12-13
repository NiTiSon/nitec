namespace NiteCompiler.CliTool;

public enum OutputKind
{
	/// <summary>
	/// <c>.nlib`</c> format with full metadata.
	/// </summary>
	nitis_lib,
	/// <summary>
	/// Native dynamic linking library.
	/// </summary>
	shared,
	/// <summary>
	/// Native static library.
	/// </summary>
	@static,
	/// <summary>
	/// Executable file.
	/// </summary>
	exec,
}