namespace NiteCompiler.Metadata;

internal sealed class TypeReferenceMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }

	internal TypeReferenceMetadata(MetadataId id, MetadataId containerId, uint nameId) : base(id)
	{
		ContainerId = containerId;
		NameId = nameId;
	}
}