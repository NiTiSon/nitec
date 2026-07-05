using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class ModuleDeclarationMetadata : MetadataEntry
{
	public MetadataId? ContainerId { get; }
	public uint NameId { get; }

	public ModuleDeclarationMetadata(MetadataId id, ModuleSymbol module, MetadataId? containerId, uint nameId) : base(id, module)
	{
		ContainerId = containerId;
		NameId = nameId;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(ContainerId is MetadataId id ? (uint)id : 0u);
		writer.Write(NameId);
	}
}