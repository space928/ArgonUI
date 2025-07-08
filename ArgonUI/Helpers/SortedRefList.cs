using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ArgonUI.Helpers;

/// <summary>
/// This is a specialised sorted list (array list) which allows values to be accessed by reference.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="T"></typeparam>
public class SortedRefList<TKey, T> : IDictionary<TKey, T>, IReadOnlyCollection<T>
{
    private TKey[]? keys;
    private T[]? items;
    private int count;
    private int version;

    public T this[TKey key]
    {
        get
        {
            if (keys == null)
                throw new IndexOutOfRangeException();
            int ind = Array.BinarySearch(keys, 0, count, key);
            if (ind < 0)
                throw new IndexOutOfRangeException();
            return items![ind];
        }
        set
        {
            if (keys == null)
                throw new IndexOutOfRangeException();
            int ind = Array.BinarySearch(keys, 0, count, key);
            if (ind < 0)
                throw new IndexOutOfRangeException();
            items![ind] = value;
        }
    }

    public int Count => count;
    public bool IsReadOnly => false;
    public ICollection<TKey> Keys => keys == null ? [] : new ArraySegment<TKey> (keys, 0, count);
    public ICollection<T> Values => items == null ? [] : new ArraySegment<T>(items, 0, count);

#if NETSTANDARD
#pragma warning disable CS8618 // Non-nulllable field must contain a non-null value when exiting the constructor.
#endif
    public SortedRefList(int capacity = 0)
    {
        Initialise(capacity);
    }
#if NETSTANDARD
#pragma warning restore CS8618
#endif

    public SortedRefList(ReadOnlySpan<(TKey, T)> items) : this(items.Length)
    {
        foreach (var item in items)
            Add(item.Item1, item.Item2);
    }

    public SortedRefList(IEnumerable<(TKey key, T value)> items) : this(0)
    {
        AddRange(items);
    }

    private void Initialise(int capacity = 0)
    {
        if (capacity != 0)
        {
            items = new T[capacity];
            keys = new TKey[capacity];
        }
    }

    public void EnsureCapacity(int capacity)
    {
#if !NETSTANDARD
        capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);
#else
        capacity = (int)PolyFill.RoundUpToPowerOf2((uint)capacity);
#endif

        if (items == null)
        {
            Initialise(capacity);
            return;
        }

        if (capacity <= items!.Length)
            return;

        var newArr = new T[capacity];
        var newKeys = new TKey[capacity];
        Array.Copy(items, newArr, count);
        Array.Copy(keys!, newKeys, count);
        var old = items;
        items = newArr;
        keys = newKeys;

#if !NETSTANDARD
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
        {
            // Clear any reference types so that the GC can reclaim them
            Array.Clear(old, 0, count);
        }
    }

    public void Add(KeyValuePair<TKey, T> item) => Add(item.Key, item.Value);

    /// <summary>
    /// Adds each element from the enumerable to the list efficiently.
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IEnumerable<(TKey key, T value)> items)
    {
        switch (items)
        {
            case ICollection<(TKey key, T value)> collection:
                {
                    EnsureCapacity(collection.Count);
                    foreach (var item in collection)
                        Add(item.key, item.value);
                    break;
                }
            default:
                {
                    foreach (var item in items)
                        Add(item.key, item.value);
                    break;
                }
        }
    }

    public void Add(TKey key, T value)
    {
        EnsureCapacity(count + 1);
        int ind = Array.BinarySearch(keys!, 0, count, key);
        if (ind < 0)
            ind = ~ind;

        Insert(ind, key, value);
    }

    /// <summary>
    /// Gets the value of an item in this sorted list at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="fromEnd"></param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public T GetValueAtIndex(int index, bool fromEnd = false)
    {
        if (fromEnd)
            index = count - index;

        if ((uint)index >= count)
            throw new IndexOutOfRangeException();

        return items![index];
    }

    /// <summary>
    /// Gets the value of an key in this sorted list at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="fromEnd"></param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public TKey GetKeyAtIndex(int index, bool fromEnd = false)
    {
        if (fromEnd)
            index = count - index;

        if ((uint)index >= count)
            throw new IndexOutOfRangeException();

        return keys![index];
    }

    public void Insert(int index, TKey key, T item)
    {
        if (index < 0 || index > count)
            throw new ArgumentOutOfRangeException(nameof(index));

        EnsureCapacity(count + 1);
        if (index == count)
            goto SetItem;

        Array.Copy(keys!, index, keys!, index + 1, count - index);
        Array.Copy(items!, index, items!, index + 1, count - index);

    SetItem:
        keys![index] = key;
        items![index] = item;
        count++;
        version++;
    }

    public void Clear()
    {
        count = 0;
        if (items != null)
        {
#if !NETSTANDARD
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
            {
                // Clear any reference types so that the GC can reclaim them
                Array.Clear(items, 0, count);
            }
        }
        version++;
    }

    public bool Remove(KeyValuePair<TKey, T> item) => Remove(item.Value);

    public bool Remove(TKey key)
    {
        if (keys == null || items == null)
            return false;

        int ind = Array.BinarySearch(keys, 0, count, key);
        if (ind < 0)
            return false;

        RemoveAt(ind);
        return true;
    }

    public bool Remove(T item)
    {
        int ind = IndexOf(item);
        if (ind == -1)
            return false;
        RemoveAt(ind);
        return true;
    }

    public void RemoveAt(int index)
    {
        if (index == count - 1)
        {
            count--;
            version++;
            return;
        }

        Array.Copy(keys!, index + 1, keys!, index, count - index - 1);
        Array.Copy(items!, index + 1, items!, index, count - index - 1);

        count--;
        version++;
    }

#if !NETSTANDARD
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
#else
    public bool TryGetValue(TKey key, out T value)
#endif
    {
        if (keys == null)
        {
            value = default!;
            return false;
        }

        int ind = Array.BinarySearch(keys, 0, count, key);
        if (ind < 0)
        {
            value = default!;
            return false;
        }

        value = items![ind];
        return true;
    }

    /// <summary>
    /// Gets a reference to the item with the given key in the list.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The item or a <see cref="Unsafe.NullRef{T}"/> if no item was found with that key.</returns>
    public ref T GetRef(TKey key)
    {
        if (keys == null)
            return ref Unsafe.NullRef<T>();

        int ind = Array.BinarySearch(keys, 0, count, key);
        if (ind < 0 || ind >= count)
            return ref Unsafe.NullRef<T>();

        return ref items![ind];
    }

    public bool Contains(T item) => Array.IndexOf(items!, item, 0, count) != -1;
    public bool ContainsKey(TKey key) => keys != null && Array.BinarySearch(keys, 0, count, key) >= 0;
    public bool Contains(KeyValuePair<TKey, T> item) => Contains(item.Value);

    /// <summary>
    /// Finds the index of a particular item in this sorted list.
    /// If the item was not found, this method returns <c>-1</c>.
    /// </summary>
    /// <seealso cref="IndexOfKey(TKey)"/>
    /// <param name="item">The item to search for in this sorted list.</param>
    /// <returns>The index of the <paramref name="item"/> or <c>-1</c> if it was not found in this list.</returns>
    public int IndexOf(T item) => Array.IndexOf(items!, item, 0, count);

    /// <summary>
    /// Finds the index of a particular key in this sorted list.
    /// If the key was not found, this method returns <c>-1</c>.
    /// </summary>
    /// <seealso cref="IndexOf(T)"/>
    /// <seealso cref="FindClosestIndex(TKey, out bool)"/>
    /// <param name="key">The key to search for in this sorted list.</param>
    /// <returns>The index of the <paramref name="key"/> or <c>-1</c> if it was not found in this list.</returns>
    public int IndexOfKey(TKey key)
    {
        var ind = Array.BinarySearch(keys!, 0, count, key);
        return ind < 0 ? -1 : ind;
    }

    /// <summary>
    /// Finds the index of the key which is closest (or equal) to the given search key.
    /// If an exact match doesn't exist, the index of the next highest key is returned.
    /// </summary>
    /// <param name="key">The key to search for.</param>
    /// <param name="exactMatch"><see langword="true"/> if <paramref name="key"/> was found in this list.</param>
    /// <returns></returns>
    public int FindClosestIndex(TKey key, out bool exactMatch)
    {
        var ind = Array.BinarySearch(keys!, 0, count, key);
        exactMatch = ind >= 0;
        return exactMatch ? ind : ~ind;
    }

    public void CopyTo(T[] array, int arrayIndex) => Array.Copy(items ?? [], 0, array, arrayIndex, count);
    public void CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex)
    {
        if (keys == null || items == null)
            return;

        for (int i = 0; i < count; i++)
            array[i + arrayIndex] = new(keys[i], items[i]);
    }

    public IEnumerator<T> GetEnumerator()
    {
        int startVersion = version;
        for (int i = 0; i < count; i++)
        {
            if (version != startVersion)
                throw new InvalidOperationException("List has been mutated since iteration began!");
            yield return items![i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<KeyValuePair<TKey, T>> IEnumerable<KeyValuePair<TKey, T>>.GetEnumerator()
    {
        int startVersion = version;
        for (int i = 0; i < count; i++)
        {
            if (version != startVersion)
                throw new InvalidOperationException("List has been mutated since iteration began!");
            yield return new(keys![i], items![i]);
        }
    }
}
