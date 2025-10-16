using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace NiteLang.Metadata;

internal sealed class Table<T> : IEnumerable<T>
	where T : ITableContent
{
	private readonly List<T> _values = [];
	private readonly Dictionary<T, Handle> _map = [];

	public int Count => _values.Count;

	public Handle Add(T item)
	{
		if (_map.TryGetValue(item, out var existing))
			return existing;

		Handle handle = MakeNextId();
		_values.Add(item);
		_map[item] = handle;
		return handle;
	}

	public T? Get(Handle handle)
	{
		int index = (int)(handle.JustValue);

		if (handle.Type != T.TableStorageType)
			return default;

		if (index < 0 || index >= _values.Count)
			return default;

		return _values[index];
	}

	private Handle MakeNextId()
	{
		return new Handle(((uint)T.TableStorageType << 28) | (uint)_values.Count);
	}

	public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}