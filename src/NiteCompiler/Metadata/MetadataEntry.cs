namespace NiteCompiler.Metadata;

public abstract class MetadataEntry
{
	public int InternalId { get; }

	private protected MetadataEntry(int internalId)
	{
		this.InternalId = internalId;
	}
}