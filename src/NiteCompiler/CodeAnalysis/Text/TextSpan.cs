using System;
using System.Runtime.Serialization;

namespace NiteCompiler.CodeAnalysis.Text;

[DataContract]
public readonly struct TextSpan
{
	[DataMember(Order = 0)] public readonly int Start;
	[DataMember(Order = 1)] public readonly int Length;
	public int End => Start + Length;
	public bool IsEmpty => this.Length == 0;

	public TextSpan(int start, int length)
	{
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, start + length);

        Start = start;
		Length = length;
	}

	public bool Contains(int position)
	{
		return unchecked((uint)(position - Start) < (uint)Length);
	}

	public bool Contains(TextSpan span)
	{
		return span.Start >= Start && span.End <= this.End;
	}

	public bool OverlapsWith(TextSpan span)
	{
		int overlapStart = int.Max(Start, span.Start);
		int overlapEnd = int.Min(End, span.End);

		return overlapStart < overlapEnd;
	}

	public TextSpan? Overlap(TextSpan span)
	{
		int overlapStart = Math.Max(Start, span.Start);
		int overlapEnd = Math.Min(this.End, span.End);

		return overlapStart < overlapEnd
			? TextSpan.FromBounds(overlapStart, overlapEnd)
			: (TextSpan?)null;
	}

	public bool IntersectsWith(TextSpan span)
	{
		return span.Start <= End && span.End >= Start;
	}

	public bool IntersectsWith(int position)
	{
		return unchecked((uint)(position - Start) <= (uint)Length);
	}

	public TextSpan? Intersection(TextSpan span)
	{
		int intersectStart = int.Max(Start, span.Start);
		int intersectEnd = int.Min(this.End, span.End);

		return intersectStart <= intersectEnd
			? TextSpan.FromBounds(intersectStart, intersectEnd)
			: (TextSpan?)null;
	}

	public static TextSpan FromBounds(int start, int end)
	{
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfLessThan(end, start);

        return new(start, end - start);
	}

	public static TextSpan FromBounds(TextSpan startFrom, TextSpan endFrom)
	{
		return new(startFrom.Start, endFrom.End - startFrom.Start);
	}

	public override string ToString() => $"[{Start}..{End})";
}