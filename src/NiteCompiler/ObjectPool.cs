using System;
using System.Diagnostics;
using System.Threading;

namespace NiteCompiler;
internal sealed class ObjectPool<T> where T : class
{
	[DebuggerDisplay("{Value,nq}")]
	private struct Element
	{
		internal T? Value;
	}

	private T? _firstItem;
	private readonly Element[] _items;
	private readonly Func<T> _factory;
	public bool TrimOnFree { get; }

	public ObjectPool(Func<T> factory, bool trimOnFree = true) : this(factory, Environment.ProcessorCount * 2, trimOnFree)
	{
	}

	public ObjectPool(Func<T> factory, int size, bool trimOnFree = true)
	{
		Debug.Assert(size >= 1);
		_factory = factory;
		_items = new Element[size - 1];
		TrimOnFree = trimOnFree;
	}

	public ObjectPool(Func<ObjectPool<T>, T> factory, int size)
	{
		Debug.Assert(size >= 1);
		_factory = () => factory(this);
		_items = new Element[size - 1];
	}

	private T CreateInstance()
	{
		return _factory();
	}

	public T Allocate()
	{
		T? inst = _firstItem;
		if (inst == null || inst != Interlocked.CompareExchange(ref _firstItem, null, inst))
		{
			inst = AllocateSlow();
		}

		return inst;
	}

	private T AllocateSlow()
	{
		var items = _items;

		for (var i = 0; i < items.Length; i++)
		{
			T? inst = items[i].Value;
			if (inst == null) continue;

			if (inst == Interlocked.CompareExchange(ref items[i].Value, null, inst))
			{
				return inst;
			}
		}

		return CreateInstance();
	}

	public void Free(T obj)
	{
		if (_firstItem == null)
		{
			_firstItem = obj;
		}
		else
		{
			FreeSlow(obj);
		}
	}

	private void FreeSlow(T obj)
	{
		var items = _items;
		for (var i = 0; i < items.Length; i++)
		{
			if (items[i].Value == null)
			{
				items[i].Value = obj;
				break;
			}
		}
	}
}