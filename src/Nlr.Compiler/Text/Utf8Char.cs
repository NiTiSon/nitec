using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Nlr.Compiler.Text;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 1)]
[DebuggerDisplay($"'{{{nameof(ToString)}(),nq}}'")]
public readonly struct Utf8Char : IComparable<Utf8Char>, IComparable<byte>, IEquatable<Utf8Char>, IEquatable<byte>
{
	public static readonly Utf8Char InvalidChar = new(0xFF);

	private readonly byte _value;

	public Utf8Char()
	{
		_value = 0;
	}

	public Utf8Char(byte value)
	{
		_value = value;
	}

	public int CompareTo(Utf8Char other)
	{
		return _value - other._value;
	}

	public int CompareTo(byte other)
	{
		return _value - other;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Utf8Char ch && Equals(ch);
	}

	public bool Equals(Utf8Char other)
	{
		return _value == other._value;
	}

	public bool Equals(byte other)
	{
		return _value == other;
	}
	public override int GetHashCode()
	{
		return _value;
	}

	public override string ToString()
	{
		return new((char)_value, 1);
	}

	public static bool operator ==(Utf8Char lhs, Utf8Char rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Utf8Char lhs, Utf8Char rhs)
	{
		return !lhs.Equals(rhs);
	}

	public static bool operator <(Utf8Char left, Utf8Char right)
	{
		return left.CompareTo(right) < 0;
	}

	public static bool operator <=(Utf8Char left, Utf8Char right)
	{
		return left.CompareTo(right) <= 0;
	}

	public static bool operator >(Utf8Char left, Utf8Char right)
	{
		return left.CompareTo(right) > 0;
	}

	public static bool operator >=(Utf8Char left, Utf8Char right)
	{
		return left.CompareTo(right) >= 0;
	}

	public static bool operator ==(Utf8Char lhs, byte rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Utf8Char lhs, byte rhs)
	{
		return !lhs.Equals(rhs);
	}

	public static bool operator <(Utf8Char left, byte right)
	{
		return left.CompareTo(right) < 0;
	}

	public static bool operator <=(Utf8Char left, byte right)
	{
		return left.CompareTo(right) <= 0;
	}

	public static bool operator >(Utf8Char left, byte right)
	{
		return left.CompareTo(right) > 0;
	}

	public static bool operator >=(Utf8Char left, byte right)
	{
		return left.CompareTo(right) >= 0;
	}

	public static implicit operator char(Utf8Char value)
	{
		return (char)value._value;
	}

	public static implicit operator Utf8Char(char value)
	{
		return Unsafe.BitCast<byte, Utf8Char>((byte)value);
	}
}
