using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class FunctionDeclarationMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }
	public FunctionBody? Body { get; set; }

	public FunctionDeclarationMetadata(MetadataId id, FunctionSymbol function, MetadataId containerId, uint nameId) : base(id, function)
	{
		ContainerId = containerId;
		NameId = nameId;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(ContainerId);
		writer.Write(NameId);
		if (Body != null)
		{
			writer.Write7BitEncodedInt(Body.IrBytes.Length);
			writer.Write(Body.IrBytes.AsSpan());
		}
	}
}