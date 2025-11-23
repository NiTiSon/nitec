using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class FreezableArray<T>
{
	private bool _isFrozen;
	private int _count;
	private ShadowImmutableArray _ice;

	public FreezableArray() : this(4) {}

	public FreezableArray(int initialCapacity)
	{
		Guard.IsGreaterThanOrEqualTo(initialCapacity, 0);
		_ice._array = new T[initialCapacity];
	}

	public static FreezableArray<T> Create() => new();

	/// <summary>
	/// Bitwise copy of <see cref="ImmutableArray{T}"/>.
	/// </summary>
	private struct ShadowImmutableArray
	{
		internal T[] _array;
	}

	public void Add(T item)
	{
		int newCount = _count + 1;
		#if DEBUG
		ThrowIfFrozen();
		#endif
		EnsureCapacity(newCount);
		_ice._array[_count] = item;
		_count = newCount;
	}

	private void EnsureCapacity(int capacity)
	{
		if (_ice._array.Length < capacity)
		{
			int newCapacity = Math.Max(_ice._array.Length * 2, capacity);
			Array.Resize(ref _ice._array, newCapacity);
		}
	}

	private void ThrowIfFrozen()
	{
		if (_isFrozen)
		{
			throw new UnreachableException();
		}
	}

	public static implicit operator ImmutableArray<T>(FreezableArray<T> array)
	{
		array._isFrozen = true;
		return Unsafe.BitCast<ShadowImmutableArray, ImmutableArray<T>>(array._ice);
	}
}