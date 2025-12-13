namespace NiteCompiler.MIR;

public struct LocalId
{
	public readonly ushort Id;

	public LocalId(ushort id)
	{
		Id = id;
	}

	/// <summary>
	/// Used for return non value.
	/// </summary>
	public static LocalId Nothing => new LocalId(ushort.MaxValue);
}