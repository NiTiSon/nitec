namespace NiteCompiler.CodeAnalysis.Syntax;

public interface INamedNode
{
	public string GetName(GetNameOptions options = GetNameOptions.Default);
}

public enum GetNameOptions
{
	Default
}