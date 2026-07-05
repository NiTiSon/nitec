using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NiteCompiler.Metadata;

internal sealed class StringTable
{
	private volatile uint _counter;
	private readonly Dictionary<string, uint> _strings;
	private readonly List<string> _orderedStrings;

	public StringTable()
	{
		_counter = 0;
		_strings = new Dictionary<string, uint>(StringComparer.Ordinal);
		_orderedStrings = [];
	}

	public uint AddOrGet(string value)
	{
		if (_strings.TryGetValue(value, out uint index))
		{
			return index;
		}

		index = Interlocked.Increment(ref _counter);
		_strings.Add(value, index);
		_orderedStrings.Add(value);
		return index;
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write7BitEncodedInt(_orderedStrings.Count);
		for (int i = 0; i < _orderedStrings.Count; i++)
		{
			writer.Write(_orderedStrings[i]);
		}
	}
}