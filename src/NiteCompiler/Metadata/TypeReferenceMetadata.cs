using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class TypeReferenceMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }
	public TypeMetadataFlags Flags { get; }

	internal TypeReferenceMetadata(MetadataId id, TypeSymbol type, MetadataId containerId, uint nameId) : base(id, type)
	{
		ContainerId = containerId;
		NameId = nameId;

		Flags = CreateFlags(type);
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(ContainerId);
		writer.Write(NameId);
		writer.Write((ushort)Flags);
		if (Flags.HasFlag(TypeMetadataFlags.IsSpecialType))
		{
			writer.Write((byte)(OriginatedFromSymbol as TypeSymbol)!.SpecialType);
		}
	}
}