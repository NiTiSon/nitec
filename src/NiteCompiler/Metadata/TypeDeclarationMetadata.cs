using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class TypeDeclarationMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }

	public TypeDeclarationMetadata(MetadataId id, TypeSymbol type, MetadataId containerId, uint nameId) : base(id, type)
	{
		ContainerId = containerId;
		NameId = nameId;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(ContainerId);
		writer.Write(NameId);
	}
}