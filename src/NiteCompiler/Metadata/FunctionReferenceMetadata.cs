namespace NiteCompiler.Metadata;

internal sealed class FunctionReferenceMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }

	public FunctionReferenceMetadata(MetadataId id, MetadataId containerId, uint nameId) : base(id)
	{
		ContainerId = containerId;
		NameId = nameId;
	}
}