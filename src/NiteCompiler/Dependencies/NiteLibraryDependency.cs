namespace NiteCompiler.Dependencies;

public sealed class NiteLibraryDependency : Dependency
{
	public string FilePath { get; }

	public override bool ContainSourceCode => false;

	public NiteLibraryDependency(string filePath)
	{
		FilePath = filePath;
	}
}
