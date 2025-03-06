using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nlr.Compiler.Text;

/// <summary>
/// An optimized representation of a substring.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly struct Utf8StringSegment : IEquatable<Utf8StringSegment>, IEquatable<Utf8String?>
{
	/// <summary>
	/// A <see cref="StringSegment"/> for <see cref="string.Empty"/>.
	/// </summary>
	public static readonly Utf8StringSegment Empty = Utf8String.Empty;

	private readonly Utf8String? _buffer;
	private readonly int _offset;
	private readonly int _length;

	public Utf8StringSegment(Utf8String? buffer)
	{
		_buffer = buffer;
		_offset = 0;
		_length = buffer?.Length ?? 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8StringSegment(Utf8String buffer, int offset, int length)
	{
		// Validate arguments, check is minimal instructions with reduced branching for inlinable fast-path
		// Negative values discovered though conversion to high values when converted to unsigned
		// Failure should be rare and location determination and message is delegated to failure functions
		if (buffer == null || (uint)offset > (uint)buffer.Length || (uint)length > (uint)(buffer.Length - offset))
		{
			ThrowHelper.ThrowArgumentException();
		}

		_buffer = buffer;
		_offset = offset;
		_length = length;
	}

	[MemberNotNullWhen(true, nameof(HasValue))]
	[MemberNotNullWhen(true, nameof(Value))]
	public Utf8String? Buffer => _buffer;

	public int Offset => _offset;

	public int Length => _length;

	public Utf8String? Value => HasValue ? Buffer.Substring(_offset, _length) : null;


	[MemberNotNullWhen(true, nameof(Buffer))]
	[MemberNotNullWhen(true, nameof(Value))]
	public bool HasValue => Buffer != null;

	public Utf8Char this[int index]
	{
		get
		{
			if ((uint)index >= (uint)Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));
			}

			Debug.Assert(Buffer is not null);
			return Buffer[_offset + index];
		}
	}

	public bool Equals(Utf8StringSegment other)
	{
		return AsSpan().SequenceEqual(other.AsSpan());
	}

	public bool Equals(Utf8String? other)
	{
		if (other is null) return false;

		return AsSpan().SequenceEqual(other.AsSpan());
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Utf8StringSegment segment && Equals(segment);
	}

	public override int GetHashCode()
	{
		return Value?.GetHashCode() ?? 0;
	}

	public ReadOnlySpan<Utf8Char> AsSpan()
	{
		return _buffer!.AsSpan().Slice(_offset, _length);
	}

	public static implicit operator Utf8StringSegment(Utf8String value)
	{
		return new(value);
	}
	public static bool operator ==(Utf8StringSegment left, Utf8StringSegment right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Utf8StringSegment left, Utf8StringSegment right)
	{
		return !(left == right);
	}
}