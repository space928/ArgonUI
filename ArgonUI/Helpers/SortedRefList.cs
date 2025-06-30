using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ArgonUI.Helpers;

public class SortedRefList<T> : IDictionary<int, T>, IReadOnlyCollection<T>
{
    private int[]? keys;
    private T[]? items;
    private int count;
    private int version;

    public T this[int key]
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
    public ICollection<int> Keys => keys == null ? [] : new ArraySegment<int> (keys, 0, count);
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

    public SortedRefList(ReadOnlySpan<(int, T)> items) : this(items.Length)
    {
        foreach (var item in items)
            Add(item.Item1, item.Item2);
    }

    public SortedRefList(IEnumerable<(int key, T value)> items) : this(0)
    {
        AddRange(items);
    }

    private void Initialise(int capacity = 0)
    {
        if (capacity != 0)
        {
            items = new T[capacity];
            keys = new int[capacity];
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
        var newKeys = new int[capacity];
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

    public void Add(KeyValuePair<int, T> item) => Add(item.Key, item.Value);

    /// <summary>
    /// Adds each element from the enumerable to the list efficiently.
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IEnumerable<(int key, T value)> items)
    {
        switch (items)
        {
            case ICollection<(int key, T value)> collection:
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

    public void Add(int key, T value)
    {
        EnsureCapacity(count + 1);
        int ind = Array.BinarySearch(keys!, 0, count, key);
        if (ind < 0)
            ind = ~ind;

        Insert(ind, key, value);
    }

    public void Insert(int index, int key, T item)
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

    public bool Remove(KeyValuePair<int, T> item) => Remove(item.Value);

    public bool Remove(int key)
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
    public bool TryGetValue(int key, [MaybeNullWhen(false)] out T value)
#else
    public bool TryGetValue(int key, out T value)
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
    public ref T GetRef(int key)
    {
        if (keys == null)
            return ref Unsafe.NullRef<T>();

        int ind = Array.BinarySearch(keys, 0, count, key);
        if (ind < 0 || ind >= count)
            return ref Unsafe.NullRef<T>();

        return ref items![ind];
    }

    public bool Contains(T item) => Array.IndexOf(items!, item, 0, count) != -1;
    public bool ContainsKey(int key) => keys != null && Array.IndexOf(keys, 0, count, key) != -1;
    public bool Contains(KeyValuePair<int, T> item) => Contains(item.Value);

    public int IndexOf(T item) => Array.IndexOf(items!, item, 0, count);

    public void CopyTo(T[] array, int arrayIndex) => Array.Copy(items ?? [], 0, array, arrayIndex, count);
    public void CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
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

    IEnumerator<KeyValuePair<int, T>> IEnumerable<KeyValuePair<int, T>>.GetEnumerator()
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
