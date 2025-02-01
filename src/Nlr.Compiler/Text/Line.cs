using System.Runtime.CompilerServices;

namespace Nlr.Compiler.Text;

/// <summary>
/// Representation of line position at file and its length.
/// </summary>
public readonly record struct Line
{
	public uint Begin { get; }

	public uint Length { get; }
	public uint DisplayLength { get; }

	public uint End => Begin + Length;

	public Line(uint begin, uint lengthWithLineBreaks, uint displayLength)
	{
		Begin = begin;
		DisplayLength = displayLength;
		Length = lengthWithLineBreaks;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool InBounds(uint characterIndex)
	{
		return Begin <= characterIndex
			&& End > characterIndex;
	}
}