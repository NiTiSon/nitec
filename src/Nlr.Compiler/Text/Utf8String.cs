using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Nlr.Compiler.Text;

// Temporal solution for Utf8String, since .NET will create their own Utf8String

[DebuggerDisplay($"\"{{{nameof(ToString)}(),nq}}\"u8")]
public sealed class Utf8String : IEquatable<ReadOnlySpan<byte>>, IEquatable<ReadOnlySpan<Utf8Char>>, IEquatable<Utf8String>
{
	public static readonly Utf8String Empty = new();

	private readonly Utf8Char[] _text;

	public Utf8String()
	{
		_text = [];
	}

	private Utf8String(Utf8Char[] text)
	{
		_text = text;
	}


	public int Length => _text.Length;

	[IndexerName("Chars")]
	public Utf8Char this[int index]
	{
		get
		{
			return _text[index];
		}
	}

	public Utf8String Substring(int offset, int length)
	{
		if (offset < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(offset));
		}
		if (offset + length > Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
		}

		Utf8Char[] subbuffer = new Utf8Char[length];

		_text.CopyTo(subbuffer, offset);
		return new(subbuffer);
	}

	public bool Equals(ReadOnlySpan<byte> other)
	{
		return _text.AsSpan().SequenceEqual(MemoryMarshal.Cast<byte, Utf8Char>(other));
	}

	public bool Equals(ReadOnlySpan<Utf8Char> other)
	{
		return _text.AsSpan().SequenceEqual(other);
	}

	public bool Equals(Utf8String? other)
	{
		if (other is null) return false;

		if (ReferenceEquals(this, other)) return true;

		if (this.Length != other.Length) return false;

		return _text.AsSpan().SequenceEqual(other._text);
	}

	public override bool Equals(object? obj)
	{
		return obj is Utf8String str && Equals(str);
	}

	public override int GetHashCode()
	{
		return _text.GetHashCode();
	}

	public override string ToString()
	{
		return Encoding.UTF8.GetString(MemoryMarshal.Cast<Utf8Char, byte>(_text.AsSpan()));
	}

	public ReadOnlySpan<Utf8Char> AsSpan()
	{
		return _text.AsSpan();
	}


	public static implicit operator Utf8String(ReadOnlySpan<Utf8Char> text)
	{
		if (text.IsEmpty)
		{
			return Empty;
		}

		Utf8Char[] buffer = new Utf8Char[text.Length];

		text.CopyTo(buffer);
		return new(buffer);
	}

	public static implicit operator Utf8String(ReadOnlySpan<byte> text)
	{
		if (text.IsEmpty)
		{
			return Empty;
		}

		Utf8Char[] buffer = new Utf8Char[text.Length];

		text.CopyTo(MemoryMarshal.Cast<Utf8Char, byte>(buffer.AsSpan()));
		return new(buffer);
	}

	public static implicit operator Utf8String(ReadOnlySpan<char> text)
	{
		if (text.IsEmpty)
		{
			return Empty;
		}

		Encoding utf8 = Encoding.UTF8;
		Utf8Char[] buffer = new Utf8Char[utf8.GetByteCount(text)];

		int read = utf8.GetBytes(text, MemoryMarshal.Cast<Utf8Char, byte>(buffer.AsSpan()));

		Debug.Assert(read == buffer.Length); // Must be the same length

		return new(buffer);
	}

	public static implicit operator Utf8String?(string? text)
	{
		if (text is null) return null;

		if (text.Length == 0) return Empty;

		Encoding utf8 = Encoding.UTF8;
		Utf8Char[] buffer = new Utf8Char[utf8.GetByteCount(text)];

		int read = utf8.GetBytes(text, MemoryMarshal.Cast<Utf8Char, byte>(buffer.AsSpan()));

		Debug.Assert(read == buffer.Length); // Must be the same length

		return new(buffer);
	}
}