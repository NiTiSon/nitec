using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NiteLang.Metadata;

public sealed class NlibLibraryBuilder : LibraryReference
{
	public override string Name { get; }
	public override SemVer Version { get; }

	private readonly Dictionary<string, NlibModuleBuilder> _modules;

	public override IEnumerable<NlibModuleBuilder> Modules => _modules.Values;
	public override IEnumerable<NlibTypeBuilder> Types => _modules.SelectMany(t => t.Value.Types);

	public NlibLibraryBuilder(string name)
	{
		Name = name;
		Version = new(1, 0, 0);
		_modules = [];
	}

	public NlibModuleBuilder CreateModule(string name = NlibConstants.GlobalModuleName)
	{
		ref NlibModuleBuilder? value = ref CollectionsMarshal.GetValueRefOrAddDefault(_modules, name, out bool exists);

		if (!exists)
		{
			value = new NlibModuleBuilder(this, name);
		}

		return value!;
	}

	internal ISet<string> CollectAllStrings()
	{
		HashSet<string> set =
		[
			Name
		];

		foreach (NlibModuleBuilder module in Modules)
		{
			set.Add(module.Name);
		}
		foreach (NlibTypeBuilder type in Types)
		{
			set.Add(type.Name);
		}

		return set;
	}
}