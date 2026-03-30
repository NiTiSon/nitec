using System;
using System.Collections.Generic;
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

	extension<T>(List<T> list)
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