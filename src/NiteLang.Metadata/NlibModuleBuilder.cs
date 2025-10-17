using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NiteLang.Metadata;

public sealed class NlibModuleBuilder : ModuleReference, ITableContent
{
	private NlibLibraryBuilder _library;
	public override string Name { get; }

	private readonly Dictionary<string, NlibTypeBuilder> _modules;

	public override IEnumerable<NlibTypeBuilder> Types => _modules.Values;

	internal NlibModuleBuilder(NlibLibraryBuilder library, string name)
	{
		_library = library;
		Name = name;
		_modules = [];
	}

	public NlibTypeBuilder CreateType(string name)
	{
		ref NlibTypeBuilder? value = ref CollectionsMarshal.GetValueRefOrAddDefault(_modules, name, out bool exists);

		if (!exists)
		{
			value = new NlibTypeBuilder(this, name);
		}

		return value!;
	}

	public static TableType TableStorageType => TableType.ModuleDeclaration;

	internal void Write(BinaryWriter writer, Table<StringConstant> strings)
	{
		Handle name = strings.Add(Name);
		writer.Write(name);
	}
}