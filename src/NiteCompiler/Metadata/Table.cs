using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NiteCompiler.Metadata;

internal sealed class Table
{
	private readonly MetadataKind _kind;
	private readonly List<MetadataEntry> _entries;

	public Table(MetadataKind kind)
	{
		_kind = kind;
		int capacity = _kind == MetadataKind.LibraryReference ? 4 : 64;

		_entries = new List<MetadataEntry>(capacity);
	}

	public MetadataEntry Get(MetadataId id)
	{
		Debug.Assert(id.Kind == _kind);

		int rawId = unchecked((int)id.Value - 1);

		return _entries[rawId];
	}

	public MetadataEntry Add(Func<MetadataId, MetadataEntry> init)
	{
		MetadataId id = GetNextMetadataId();

		MetadataEntry entry = init(id);

		_entries.Add(entry);

		return entry;
	}

	private MetadataId GetNextMetadataId()
	{
		int count = _entries.Count;
		return new MetadataId(_kind, (uint)count + 1);
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write((byte)_kind);
		writer.Write7BitEncodedInt(_entries.Count);
		for (int i = 0; i < _entries.Count; i++)
		{
			MetadataEntry entry = _entries[i];

			entry.Write(writer);
		}
	}
}