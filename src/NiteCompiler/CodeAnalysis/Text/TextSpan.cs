using System;
using System.Runtime.Serialization;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Text;

/// <summary>
/// Span of text.
/// </summary>
[DataContract]
public readonly struct TextSpan : IEquatable<TextSpan>, IComparable<TextSpan>, IComparable
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

	public Location Contextualize(SyntaxTree tree)
	{
		return Location.Create(tree, this);
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

	public bool Equals(TextSpan other)
	{
		return Start == other.Start && Length == other.Length;
	}

	public override bool Equals(object? obj)
	{
		return obj is TextSpan other && Equals(other);
	}

	public int CompareTo(object? obj)
	{
		if (obj is null) return 1;
		return obj is TextSpan other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TextSpan)}");
	}

	public int CompareTo(TextSpan other)
	{
		var startComparison = Start.CompareTo(other.Start);
		if (startComparison != 0) return startComparison;
		return Length.CompareTo(other.Length);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Start, Length);
	}

	public static bool operator ==(TextSpan left, TextSpan right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TextSpan left, TextSpan right)
	{
		return !(left == right);
	}

	public static bool operator <(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) < 0;
	}

	public static bool operator >(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) > 0;
	}

	public static bool operator <=(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) <= 0;
	}

	public static bool operator >=(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) >= 0;
	}
}