namespace NiteCompiler.IntermediateRepresentation;

internal readonly struct BlockId
{
	public readonly ushort Id;

	public BlockId(ushort id)
	{
		Id = id;
	}
}