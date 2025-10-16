using System.Collections.Generic;
using System.Linq;

namespace NiteLang.Metadata;

public abstract class LibraryReference
{
	public abstract string Name { get; }
	public abstract SemVer Version { get; }
	public abstract IEnumerable<ModuleReference> Modules { get; }
	public virtual IEnumerable<TypeReference> Types => Modules.SelectMany(t => t.Types);
}