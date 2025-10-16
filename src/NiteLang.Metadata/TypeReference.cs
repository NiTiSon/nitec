namespace NiteLang.Metadata;

public abstract class TypeReference
{
	public abstract ModuleReference Module { get; }
	public abstract string Name { get; }
}

public sealed class NlibTypeBuilder : TypeReference
{
	public override ModuleReference Module { get; }
	public override string Name { get; }

	internal NlibTypeBuilder(NlibModuleBuilder module, string name)
	{
		Module = module;
		Name = name;
	}
}