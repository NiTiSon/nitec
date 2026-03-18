using System.IO;

namespace NiteCompiler.Metadata;

internal sealed class LibraryReferenceMetadata : MetadataEntry
{
	public uint NameId { get; }

	public LibraryReferenceMetadata(MetadataId id, uint nameId) : base(id)
	{
		NameId = nameId;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(NameId);
	}
}