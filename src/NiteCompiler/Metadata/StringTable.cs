using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NiteCompiler.Metadata;

/// <summary>
/// Strings has its own unique table, separated from other metadata entries.
/// </summary>
internal sealed class StringTable
{
	private volatile uint _counter;
	private readonly Dictionary<string, uint> _strings;

	public StringTable()
	{
		_counter = 0;
		_strings = new Dictionary<string, uint>(StringComparer.Ordinal);
	}

	public uint AddOrGet(string value)
	{
		if (_strings.TryGetValue(value, out uint index))
		{
			return index;
		}
		else
		{
			index = Interlocked.Increment(ref _counter);
			_strings.Add(value, index);
			return index;
		}
	}

	public void Write(BinaryWriter writer)
	{
		var strings = _strings.Keys;
		foreach (string str in strings)
		{
			writer.Write(str);
		}
	}
}