namespace Nlr.Compiler;

public abstract class CommonCompiler
{
	private readonly BuildPaths _buildPaths;

	private protected CommonCompiler(BuildPaths buildPaths)
	{
		_buildPaths = buildPaths;
	}

	public abstract Compilation? CreateCompilation(string[] sourceFiles);
}