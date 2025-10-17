using System.IO;

namespace NiteLang.Metadata;

public abstract class FunctionReference
{
	public abstract string Name { get; }
}

public sealed class NlibFunctionBuilder : FunctionReference, ITableContent
{
	private object _parent;

	public override string Name { get; }

	internal NlibFunctionBuilder(NlibModuleBuilder module, string name)
	{
		_parent = module;
		Name = name;
	}

	internal NlibFunctionBuilder(NlibTypeBuilder module, string name)
	{
		_parent = module;
		Name = name;
	}

	public NlibModuleBuilder? ContainerModule => _parent as NlibModuleBuilder;
	public NlibTypeBuilder? ContainerType => _parent as NlibTypeBuilder;

	public static TableType TableStorageType => TableType.FunctionDeclaration;

	internal void Write(BinaryWriter writer, Table<NlibModuleBuilder> modules, Table<NlibTypeBuilder> types, Table<StringConstant> strings)
	{
		Handle name = strings.Add(Name);
		Handle parent = Handle.Null;
		if (ContainerModule is not null) parent = modules.Add(ContainerModule);
		else if (ContainerType is not null) parent = types.Add(ContainerType);
		writer.Write(name);
		writer.Write(parent);
	}
}