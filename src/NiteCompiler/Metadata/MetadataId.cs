using System.Diagnostics;

namespace NiteCompiler.Metadata;

/// <summary>
/// Structure that stores metadata kind and unique id within its table.
/// Maximum unique values is 16'777'214, what's big enough for anything.
/// </summary>
internal readonly struct MetadataId
{
	private const int KindOffset = 24;

	private readonly uint _bits;

	public MetadataId(uint rawValue)
	{
		_bits = rawValue;
	}

	public MetadataId(MetadataKind kind, uint id)
	{
		id |= (uint)kind << KindOffset;
		_bits = id;
		Debug.Assert(Kind == kind);
	}

	public MetadataKind Kind => (MetadataKind)(_bits << KindOffset);

	public uint Value => _bits & 0x00FFFFFF;

	/// <summary>
	/// Returns <see langword="true"/> when identifier is equals zero; otherwise <see langword="false"/>.
	/// </summary>
	public bool IsNull => Value == 0;

	public static implicit operator uint(MetadataId id) => id.Value;
	public static implicit operator MetadataId(uint rawValue) => new(rawValue);
}