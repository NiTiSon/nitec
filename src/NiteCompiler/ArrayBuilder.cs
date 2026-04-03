using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace NiteCompiler;

[DebuggerDisplay("Count = {Count,nq}")]
[DebuggerTypeProxy(typeof(ArrayBuilder<>.DebuggerProxy))]
public sealed class ArrayBuilder<T> : ICollection<T>
{
	private const int PooledArrayLengthLimit = 64;
	private static readonly ObjectPool<ArrayBuilder<T>> Pool = CreatePool();

	private sealed class DebuggerProxy
	{
		private readonly ArrayBuilder<T> _builder;

		public DebuggerProxy(ArrayBuilder<T> builder)
		{
			_builder = builder;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				var result = new T[_builder.Count];
				for (var i = 0; i < result.Length; i++)
				{
					result[i] = _builder[i];
				}

				return result;
			}
		}
	}

	private readonly ImmutableArray<T>.Builder _builder;
	private readonly ObjectPool<ArrayBuilder<T>>? _pool;

	public ArrayBuilder(int size)
	{
		_builder = ImmutableArray.CreateBuilder<T>(size);
	}

	public ArrayBuilder()
		: this(8)
	{ }

	private ArrayBuilder(ObjectPool<ArrayBuilder<T>> pool)
		: this()
	{
		_pool = pool;
	}

	public int Count
	{
		get => _builder.Count;
		set => _builder.Count = value;
	}

	public int Capacity
	{
		get => _builder.Capacity;
		set => _builder.Capacity = value;
	}

	public T this[int index]
	{
		get => _builder[index];
		set => _builder[index] = value;
	}

	public bool IsReadOnly
		=> false;

	public bool IsEmpty
		=> Count == 0;

	public void Add(T item)
	{
		_builder.Add(item);
	}

	public void AddRange<U>(ArrayBuilder<U> items) where U : T
	{
		_builder.AddRange(items._builder);
	}

	public void AddRange(params ImmutableArray<T> items)
	{
		_builder.AddRange(items);
	}

	public void AddRange(IEnumerable<T> items)
	{
		_builder.AddRange(items);
	}

	public void AddRange(params T[] items)
	{
		_builder.AddRange(items);
	}

	public void Insert(int index, T item)
	{
		_builder.Insert(index, item);
	}

	public void EnsureCapacity(int capacity)
	{
		if (_builder.Capacity < capacity)
		{
			_builder.Capacity = capacity;
		}
	}

	public void Clear()
	{
		_builder.Clear();
	}

	public bool Contains(T item)
	{
		return _builder.Contains(item);
	}

	public int IndexOf(T item)
	{
		return _builder.IndexOf(item);
	}

	public int IndexOf(T item, IEqualityComparer<T> equalityComparer)
	{
		return _builder.IndexOf(item, 0, _builder.Count, equalityComparer);
	}

	public int IndexOf(T item, int startIndex, int count)
	{
		return _builder.IndexOf(item, startIndex, count);
	}

	public int FindIndex(Predicate<T> match)
		=> FindIndex(0, this.Count, match);

	public int FindIndex(int startIndex, Predicate<T> match)
		=> FindIndex(startIndex, this.Count - startIndex, match);

	public int FindIndex(int startIndex, int count, Predicate<T> match)
	{
		var endIndex = startIndex + count;
		for (var i = startIndex; i < endIndex; i++)
		{
			if (match(_builder[i]))
			{
				return i;
			}
		}

		return -1;
	}

	public bool Remove(T element)
	{
		return _builder.Remove(element);
	}

	public void RemoveAt(int index)
	{
		_builder.RemoveAt(index);
	}

	public void RemoveRange(int index, int length)
	{
		_builder.RemoveRange(index, length);
	}

	public void RemoveLast()
	{
		_builder.RemoveAt(_builder.Count - 1);
	}

	public void Reverse()
	{
		_builder.Reverse();
	}

	public void Sort()
	{
		_builder.Sort();
	}

	public void Sort(IComparer<T>? comparer)
	{
		_builder.Sort(comparer);
	}

	public void Sort(Comparison<T> compare)
	{
		if (Count <= 1)
			return;

		Sort(Comparer<T>.Create(compare));
	}

	public void Sort(int startIndex, IComparer<T> comparer)
	{
		_builder.Sort(startIndex, _builder.Count - startIndex, comparer);
	}

	public T[] ToArray()
	{
		return _builder.ToArray();
	}

	public T[] ToArrayAndFree()
	{
		T[] result = _builder.ToArray();
		Free();
		return result;
	}

	public void CopyTo(T[] array, int start)
	{
		_builder.CopyTo(array, start);
	}

	public T Last() => _builder[^1];

	public T? LastOrDefault() => Count == 0 ? default : Last();

	public T First() => _builder[0];

	public T? FirstOrDefault() => Count == 0 ? default : First();

	public bool Any()
	{
		return _builder.Count > 0;
	}

	public bool Any(Predicate<T> predicate)
	{
		for (int i = 0; i < Count; i++)
		{
			if (predicate(_builder[i]))
			{
				return true;
			}
		}

		return false;
	}

	public ImmutableArray<T> ToImmutable()
	{
		return _builder.ToImmutable();
	}

	public ImmutableArray<T> ToImmutableOrDefault()
	{
		if (Count == 0)
		{
			return default;
		}

		return ToImmutable();
	}

	public ImmutableArray<T> ToImmutableAndFree()
	{
		ImmutableArray<T> result;
		if (Count == 0)
		{
			result = ImmutableArray<T>.Empty;
		}
		else if (_builder.Capacity == Count)
		{
			result = _builder.MoveToImmutable();
		}
		else
		{
			result = ToImmutable();
		}

		Free();
		return result;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _builder.GetEnumerator();
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _builder.GetEnumerator();
	}

	public void Free()
	{
		var pool = _pool;
		if (pool != null)
		{
			if (_builder.Capacity >= PooledArrayLengthLimit) return; // just don't return object to the pool, let GC kill that ArrayBuilder

			if (Count != 0)
			{
				Clear();
			}

			pool.Free(this);
		}
	}

	public static ArrayBuilder<T> GetInstance()
	{
		var builder = Pool.Allocate();
		Debug.Assert(builder.Count == 0);
		return builder;
	}

	public static ArrayBuilder<T> GetInstance(int capacity)
	{
		var builder = GetInstance();
		builder.EnsureCapacity(capacity);
		return builder;
	}

	public static ArrayBuilder<T> GetInstance(int capacity, T fillWithValue)
	{
		var builder = GetInstance();
		builder.EnsureCapacity(capacity);

		for (var i = 0; i < capacity; i++)
		{
			builder.Add(fillWithValue);
		}

		return builder;
	}

	internal static ObjectPool<ArrayBuilder<T>> CreatePool()
	{
		return CreatePool(64);
	}

	internal static ObjectPool<ArrayBuilder<T>> CreatePool(int size)
	{
		ObjectPool<ArrayBuilder<T>>? pool = null;
		pool = new ObjectPool<ArrayBuilder<T>>(() => new ArrayBuilder<T>(pool), size);
		return pool;
	}
}