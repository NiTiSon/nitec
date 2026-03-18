namespace NiteCompiler.Metadata;

internal sealed class FunctionDeclarationMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }
	public byte[]? Body { get; }

	public FunctionDeclarationMetadata(MetadataId id, MetadataId containerId, uint nameId, byte[]? compiledBody = null) : base(id)
	{
		ContainerId = containerId;
		NameId = nameId;
		Body = compiledBody;
	}
}