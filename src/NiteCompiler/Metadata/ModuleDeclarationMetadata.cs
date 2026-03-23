using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal sealed class ModuleDeclarationMetadata : MetadataEntry
{
	public uint NameId { get; }

	public ModuleDeclarationMetadata(MetadataId id, ModuleSymbol module, uint nameId) : base(id, module)
	{
		NameId = nameId;
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(NameId);
	}
}