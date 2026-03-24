using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

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

	protected static TypeMetadataFlags CreateFlags(TypeSymbol type)
	{
		TypeMetadataFlags result = 0;

		if (type.SpecialType != SpecialType.None)
		{
			result |= TypeMetadataFlags.IsSpecialType;
		}

		return result;
	}

	public abstract void Write(BinaryWriter writer);
}