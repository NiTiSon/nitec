using System;
using System.Collections.Generic;

namespace NiteCompiler;

public static class CollectionExtensions
{
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