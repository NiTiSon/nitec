using System.Collections.Immutable;
using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class FunctionDeclarationMetadata : MetadataEntry
{
	public MetadataId ContainerId { get; }
	public uint NameId { get; }
	public ImmutableArray<ParameterEntry> Parameters { get; }
	public FunctionBody? Body { get; set; }

	public FunctionDeclarationMetadata(MetadataId id, FunctionSymbol function, MetadataId containerId, uint nameId, ImmutableArray<ParameterEntry> parameters) : base(id, function)
	{
		ContainerId = containerId;
		NameId = nameId;
		Parameters = parameters;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(ContainerId);
		writer.Write(NameId);
		writer.Write7BitEncodedInt(Parameters.Length);
		foreach (var param in Parameters)
		{
			writer.Write(param.TypeId);
			writer.Write(param.NameId);
		}

		if (Body != null)
		{
			writer.Write((byte)1);
			writer.Write7BitEncodedInt(Body.IrBytes.Length);
			writer.Write(Body.IrBytes.AsSpan());
		}
		else
		{
			writer.Write((byte)0);
		}
	}
}
