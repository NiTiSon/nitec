using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class LibraryReferenceMetadata : MetadataEntry
{
	public uint NameId { get; }

	public LibraryReferenceMetadata(MetadataId id, LibrarySymbol library, uint nameId) : base(id, library)
	{
		NameId = nameId;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(NameId);
	}
}