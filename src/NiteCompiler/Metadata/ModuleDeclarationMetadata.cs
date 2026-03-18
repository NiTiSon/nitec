namespace NiteCompiler.Metadata;

internal sealed class ModuleDeclarationMetadata : MetadataEntry
{
	public uint NameId { get; }

	public ModuleDeclarationMetadata(MetadataId id, uint nameId) : base(id)
	{
		NameId = nameId;
	}
}