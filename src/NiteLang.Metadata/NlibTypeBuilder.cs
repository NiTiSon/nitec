using System.IO;

namespace NiteLang.Metadata;

public sealed class NlibTypeBuilder : TypeReference, ITableContent
{
	public override NlibModuleBuilder Module { get; }
	public override string Name { get; }

	internal NlibTypeBuilder(NlibModuleBuilder module, string name)
	{
		Module = module;
		Name = name;
	}

	public static TableType TableStorageType => TableType.ModuleDeclaration;

	internal void Write(BinaryWriter writer, Table<NlibModuleBuilder> modules, Table<StringConstant> strings)
	{
		Handle name = strings.Add(Name);
		writer.Write(name);
		writer.Write(modules.Add(Module));
	}
}