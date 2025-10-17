namespace NiteLang.Metadata;

public abstract class TypeReference
{
	public abstract ModuleReference Module { get; }
	public abstract string Name { get; }
}