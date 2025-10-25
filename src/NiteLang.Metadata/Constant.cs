using CommunityToolkit.Diagnostics;

namespace NiteLang.Metadata;

public sealed class Constant : ITableContent
{
	private readonly byte[] _data;
	private readonly NlrType _type;

	public Constant(byte[] data, NlrType type)
	{
		Guard.IsNotNull(type);
		Guard.IsNotNull(data);
		_data = data;
		_type = type;
	}

	public static TableType StorageType =>  TableType.Constant;
}