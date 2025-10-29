using System;
using System.Runtime.Serialization;
using CommunityToolkit.Diagnostics;

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
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Start = start;
		Length = length;
	}

	public bool Contains(int position)
	{
		return unchecked((uint)(position - Start) < (uint)Length);
	}

	public bool Contains(TextSpan span)
	{
		return span.Start >= Start && span.End <= End;
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
		int overlapEnd = Math.Min(End, span.End);

		return overlapStart < overlapEnd
			? FromBounds(overlapStart, overlapEnd)
			: null;
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
		int intersectEnd = int.Min(End, span.End);

		return intersectStart <= intersectEnd
			? FromBounds(intersectStart, intersectEnd)
			: null;
	}

	public TextSpan First(int length) => SubSpan(0, length);

	public TextSpan Last(int length)
	{
		if (length > Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
		}

		return SubSpan(Length - length, length);
	}

	public TextSpan SubSpan(int position, int length)
	{
		if ((uint)position > (uint)Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(position));
		}

		if ((uint)length > (uint)(Length - position))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
		}

		return new(Start + position, length);
	}

	public SourceSpan Contextualize(SourceText source)
	{
		return new(source, this);
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