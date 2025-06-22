using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Helpers;

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

    /// <summary>
    /// Returns a <see cref="OneEnumerable{T}"/> of this object if it is not <see langword="null"/>.
    /// If this object is <see langword="null"/>, it returns an <see cref="Enumerable.Empty{TResult}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static IEnumerable<T> AsOneEnumerableOrEmpty<T>(this T? item)
    {
        return item == null ? Enumerable.Empty<T>() : new OneEnumerable<T>(item);
    }

    /// <summary>
    /// Returns a <see cref="OneEnumerable{T}"/> of this object if it is not <see langword="null"/>.
    /// If this object is <see langword="null"/>, then this method returns <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static IEnumerable<T>? AsOneEnumerable<T>(this T? item)
    {
        return item == null ? null : new OneEnumerable<T>(item);
    }
}
