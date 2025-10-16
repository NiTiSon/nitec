namespace NiteLang.Metadata;

public readonly struct StringConstant : ITableContent
{
	public string Value { get; }

	public StringConstant(string value)
	{
		Value = value;
	}

	public static TableType TableStorageType => TableType.StringTable;

	public static implicit operator string(StringConstant constant)
	{
		return constant.Value;
	}

	public static implicit operator StringConstant(string value)
	{
		return new(value);
	}
}