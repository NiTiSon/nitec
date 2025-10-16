using System;

namespace NiteLang.Metadata;

public readonly struct Handle : IEquatable<Handle>
{
	private readonly uint _handle;

	public Handle(uint handle) => _handle = handle;

	public TableType Type => (TableType)(_handle & 0xF0000000);

	public uint Value => _handle;

	public uint JustValue => _handle & ~0xF0000000;

	public bool Equals(Handle other)
	{
		return _handle == other._handle;
	}

	public override bool Equals(object? obj)
	{
		return obj is Handle other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (int)_handle;
	}

	public override string ToString()
	{
		return "0x" + _handle.ToString("X");
	}

	public static implicit operator uint(Handle handle)
	{
		return handle._handle;
	}
}