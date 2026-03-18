using System.IO;

namespace NiteCompiler.Metadata;

internal abstract class MetadataEntry
{
	public MetadataId Id { get; }

	private protected MetadataEntry(MetadataId id)
	{
		this.Id = id;
	}

	public abstract void Write(BinaryWriter writer);
}