using System.Diagnostics;
using System.IO;

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

	public override void Write(BinaryWriter writer)
	{
		writer.Write(ContainerId);
		writer.Write(NameId);
		if (Body != null)
		{
			writer.Write7BitEncodedInt(Body.Length);
			writer.Write(Body);
		}
	}
}