using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NiteCompiler;

public static class CollectionExtensions
{
	extension<T>(ReadOnlySpan<T> span)
	{
		public bool All(Predicate<T> condition)
		{
			for (int i = 0;  i < span.Length; i++)
			{
				if (!condition(span[i]))
				{
					return false;
				}
			}

			return true;
		}
	}

	extension<T>(IEnumerable<T> items)
	{
		public Dictionary<TKey, T> MakeDictionaryFromValues<TKey>(Func<T, TKey> keyAccess)
			where TKey : notnull
		{
			Dictionary<TKey, T> dictionary = new();
			foreach (T item in items)
			{
				dictionary[keyAccess(item)] = item;
			}

			return dictionary;
		}

		public FrozenDictionary<TKey, T> MakeFrozenDictionaryFromValues<TKey>(Func<T, TKey> keyAccess)
			where TKey : notnull
		{
			Dictionary<TKey, T> dictionary = new();
			foreach (T item in items)
			{
				dictionary[keyAccess(item)] = item;
			}

			return dictionary.ToFrozenDictionary();
		}
	}

	extension<T>(IEnumerable<T> keys)
		where T : notnull
	{
		public Dictionary<T, TValue> MakeDictionaryFromKeys<TValue>(Func<T, TValue> valueAccess)
		{
			Dictionary<T, TValue> dictionary = new();
			foreach (T key in keys)
			{
				dictionary[key] = valueAccess(key);
			}

			return dictionary;
		}

		public FrozenDictionary<T, TValue> MakeFrozenDictionaryFromKeys<TValue>(Func<T, TValue> valueAccess)
		{
			Dictionary<T, TValue> dictionary = new();
			foreach (T key in keys)
			{
				dictionary[key] = valueAccess(key);
			}

			return dictionary.ToFrozenDictionary();
		}
	}

	extension<T>(ICollection<T> list)
	{
		public bool AddNotNull([NotNullWhen(true)] T? item)
		{
			if (item == null)
			{
				return false;
			}

			list.Add(item);
			return true;
		}
	}

    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        public TValue GetOrAdd(TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out var existingValue))
            {
                return existingValue;
            }
            else
            {
                dictionary.Add(key, value);
                return value;
            }
        }

        public TValue GetOrAdd(TKey key, Func<TValue> getValue)
        {
            if (dictionary.TryGetValue(key, out var existingValue))
            {
                return existingValue;
            }
            else
            {
                var value = getValue();
                dictionary.Add(key, value);
                return value;
            }
        }
    }
}