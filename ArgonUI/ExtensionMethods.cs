using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI;

public static partial class ExtensionMethods
{
    /// <summary>
    /// Gets a value with a given key from the dictionary, or if it doesn't exist, calls the factory method and 
    /// adds the new item to the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns>The newly added value.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, in TKey key, Func<TValue> factory) where TKey : notnull
    {
        if (dict.TryGetValue(key, out TValue? value))
            return value;

        value = factory();
        dict.Add(key, value);
        return value;
    }
}
