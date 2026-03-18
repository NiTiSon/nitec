namespace NiteCompiler.Metadata;

internal sealed class TypeDeclarationMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }

	public TypeDeclarationMetadata(MetadataId id, MetadataId containerId, uint nameId) : base(id)
	{
		ContainerId = containerId;
		NameId = nameId;
	}
}