using System.Collections.Generic;

namespace NiteLang.Metadata;

public abstract class ModuleReference
{
	public abstract string Name { get; }
	public abstract IEnumerable<TypeReference> Types { get; }
}