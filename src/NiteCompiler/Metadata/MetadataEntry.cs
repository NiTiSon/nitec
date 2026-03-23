using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Metadata;

internal abstract class MetadataEntry
{
	public MetadataId Id { get; }
	public Symbol? OriginatedFromSymbol { get; }

	private protected MetadataEntry(MetadataId id, Symbol? originatedFromSymbol)
	{
		this.Id = id;
		OriginatedFromSymbol = originatedFromSymbol;
	}

	public abstract void Write(BinaryWriter writer);
}