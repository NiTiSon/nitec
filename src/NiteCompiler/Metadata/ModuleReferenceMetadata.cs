using System.Diagnostics;
using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class ModuleReferenceMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public MetadataId LibraryId { get; }
	public uint NameId { get; }

	public ModuleReferenceMetadata(MetadataId id, ModuleSymbol module, MetadataId containerId, MetadataId libraryId, uint nameId) : base(id, module)
	{
		Debug.Assert(libraryId.Kind == MetadataKind.LibraryReference);
		ContainerId = containerId;
		LibraryId = libraryId;
		NameId = nameId;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(ContainerId);
		writer.Write(LibraryId);
		writer.Write(NameId);
	}
}