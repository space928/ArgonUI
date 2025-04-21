using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ArgonUI;

[DebuggerDisplay("Count = {Count}")]
public class ObservableStringSet : ICollection<string>, ISet<string>, IReadOnlyCollection<string>, INotifyCollectionChanged//, IReadOnlySet<string>
{
    private string[] array;
    private int[] hashes;
    private int[] buckets;
    private int count;
    private int version;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public ObservableStringSet() : this(0)
    {

    }

    public ObservableStringSet(int capacity)
    {
        array = new string[capacity];
        hashes = new int[capacity];
#if !NETSTANDARD
        buckets = new int[BitOperations.RoundUpToPowerOf2((uint)(capacity + 1))];
#else
        buckets = new int[PolyFill.RoundUpToPowerOf2((uint)(capacity + 1))];
#endif
        count = 0;
    }

    public ObservableStringSet(ICollection<string> collection) : this(collection.Count)
    {
        AddRange(collection);
    }

    public ObservableStringSet(IEnumerable<string> collection) : this(8)
    {
        AddRange(collection);
    }

    public ObservableStringSet(params string[] collection) : this(collection.Length)
    {
        AddRange(collection);
    }

    public string this[int index]
    {
        get
        {
            if (index >= count)
                throw new IndexOutOfRangeException();
            return array[index];
        }
        set
        {
            if (index >= count)
                throw new IndexOutOfRangeException();
            ref var item = ref array[index];
            var prev = item;
            item = value;

            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, value, prev));
        }
    }

    public int Count => count;
    public bool IsReadOnly => false;
    public int Capacity => array.Length;

    private void Grow()
    {
#if !NETSTANDARD
        int size = (int)BitOperations.RoundUpToPowerOf2((uint)(array.Length + 1));
#else
        int size = (int)PolyFill.RoundUpToPowerOf2((uint)(array.Length + 1));
#endif
        Resize(size);
    }

    private static int GetKeyHash(ReadOnlySpan<char> key)
    {
        //return string.GetHashCode(key);

        int hash = unchecked((int)2166136261);
        const int prime = 16777619;

        /*var bytes = MemoryMarshal.AsBytes(key);
        for (int i = 0; i < bytes.Length; i++)
        {
            hash ^= bytes[i];
            hash *= prime;
        }*/

        // FNV Hash, a very simple hash, but in my testing performe equally well in quality to the
        // default hash (the load factor and total number of buckets with links are almost identical),
        // in fact FNV was sometimes even slightly better. And it's always slightly faster.
        int len = key.Length;
#if !NETSTANDARD
        var ints = MemoryMarshal.CreateReadOnlySpan(
#else
        var ints = PolyFill.CreateReadOnlySpan(
#endif
            ref Unsafe.As<char, int>(ref MemoryMarshal.GetReference(key)),
            len << 1);//MemoryMarshal.Cast<char, int>(key);
        int lenInt = len >> 1;
        for (int i = 0; i < lenInt; i++)
        {
            hash ^= ints[i];
            hash *= prime;
        }
        if ((len & 1) == 1)
        {
            hash ^= key[len - 1];
            hash *= prime;
        }

        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FastMod(int hash, int mod)
    {
        // https://lemire.me/blog/2016/06/27/a-fast-alternative-to-the-modulo-reduction/
        //return unchecked((int)(((ulong)(uint)hash * (ulong)mod) >> 32));
        // Only works for mods which are powers of 2.
        return hash & (mod - 1);
    }

    private ref int FindSlot(ReadOnlySpan<char> item, int hash)
    {
        int nBuckets = buckets.Length;
        int slot = FastMod(hash, nBuckets);
        int cand = slot;
        do
        {
            int ind = buckets[cand];
            if (ind != 0)
            {
                // Buckets indices are 1-based
                if (hash == hashes[ind - 1]
                    && item.SequenceEqual(array[ind - 1].AsSpan()))
                    return ref buckets[cand];
            }
            else
            {
                return ref Unsafe.NullRef<int>();
            }

            cand++;
            if (cand == nBuckets)
                cand = 0;
        } while (cand != slot);

        return ref Unsafe.NullRef<int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref int FindSlot(ReadOnlySpan<char> item)
    {
        int keyHash = GetKeyHash(item);
        return ref FindSlot(item, keyHash);
    }

    private bool AddToSet(int hash, int index)
    {
        int nBuckets = buckets.Length;
        int slot = FastMod(hash, nBuckets);
        int cand = slot;
        do
        {
            if (buckets[cand] == 0)
            {
                buckets[cand] = index + 1; // Indices in the buckets array are 1-based such that 0 can be used as a sentinel.
                return true;
            }
            cand++;
            if (cand == nBuckets)
                cand = 0;
        } while (cand != slot);

        return false;
    }

    /// <inheritdoc cref="FindRelated(int)"/>
    /// <param name="item">The item who's bucket will be returned.</param>
    public IEnumerable<int> FindRelated(ReadOnlySpan<char> item)
    {
        return FindRelated(GetKeyHash(item));
    }

    /// <inheritdoc cref="FindRelated(int)"/>
    /// <param name="item">The item who's bucket will be returned.</param>
    public IEnumerable<int> FindRelated(string item)
    {
        return FindRelated(GetKeyHash(item.AsSpan()));
    }

    /// <summary>
    /// Returns an <see cref="IEnumerable{T}"/> of the indexes of all items in the bucket for the given item.
    /// </summary>
    /// <remarks>
    /// This is useful for fuzzy searching for items which share a hash code.
    /// </remarks>
    /// <param name="hash">The hash of the item who's bucket will be returned.</param>
    /// <returns></returns>
    public IEnumerable<int> FindRelated(int hash)
    {
        int slot = FastMod(hash, buckets.Length);
        int cand = slot;
        do
        {
            int ind = buckets[cand];
            if (ind != 0)
            {
                yield return ind - 1;
            }
            else
            {
                yield break;
            }

            cand++;
            if (cand == buckets.Length)
                cand = 0;
        } while (cand != slot);

        yield break;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefillBuckets()
    {
        for (int i = 0; i < count; i++)
        {
            //var item = array[i];
            //var hash = GetKeyHash(item.AsSpan());
            var hash = hashes[i];
            AddToSet(hash, i);
        }
    }

    public void Resize(int capacity)
    {
        if (capacity == array.Length)
            return;

        if (capacity < count)
            capacity = count;

        var nArray = new string[capacity];
        Array.Copy(array, nArray, count);
        array = nArray;

        var nHashes = new int[capacity];
        Array.Copy(hashes, nHashes, count);
        hashes = nHashes;

#if !NETSTANDARD
        var nBuckets = new int[(int)BitOperations.RoundUpToPowerOf2((uint)(capacity + 1))];
#else
        var nBuckets = new int[(int)PolyFill.RoundUpToPowerOf2((uint)(capacity + 1))];
#endif
        buckets = nBuckets;
        RefillBuckets();

        version++;
    }

    public bool Add(string item)
    {
        bool res = AddSilent(item);
        if (res)
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item));
        return res;
    }

    private bool AddSilent(string item)
    {
        var itemHash = GetKeyHash(item.AsSpan());
        return AddSilent(item, itemHash);
    }

    private bool AddSilent(string item, int hash)
    {
        if (count >= array.Length)
            Grow();

        //if (Contains(in item))
        //    return false;

        version++;

        hashes[count] = hash;
        array[count] = item;
        if (AddToSet(hash, count))
        {
            count++;
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item));
            return true;
        }

        // No free slot found, this shouldn't happen...
        return false;
    }

    /// <summary>
    /// Attempts to add multiple items to this set.
    /// </summary>
    /// <param name="items">The items to add to the set.</param>
    /// <returns><see langword="true"/> if all the items were added to the set.</returns>
    public bool AddRange(IEnumerable<string> items)
    {
        if (items is ICollection<string> collection)
        {
            int ncap = Math.Max(Capacity, count + collection.Count);
#if !NETSTANDARD
            ncap = (int)BitOperations.RoundUpToPowerOf2((uint)(ncap + 1));
#else
            ncap = (int)PolyFill.RoundUpToPowerOf2((uint)(ncap + 1));
#endif
            Resize(ncap);
        }

        bool allAdded = true;
        if (items is ObservableStringSet stringSet)
        {
            // Fast path for string sets to avoid additional hashing
            for (int i = 0; i < stringSet.count; i++)
            {
                var item = stringSet.array[i];
                var hash = stringSet.hashes[i];
                allAdded &= AddSilent(item, hash);
            }
        }
        else
        {
            foreach (var item in items)
                allAdded &= AddSilent(item);
        }
        // TODO: This is not correct as some items might not have been added.
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, items));
        return allAdded;
    }

    void ICollection<string>.Add(string item) => Add(item);

    public void Clear()
    {
        Array.Clear(array, 0, count);
        //Array.Clear(hashes, 0, count); // Doesn't contain any references, no need to clear it.
        Array.Clear(buckets, 0, buckets.Length);
        count = 0;
        version++;

        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
    }

    public bool RemoveRange(IEnumerable<string> items)
    {
        if (items is ObservableStringSet otherSet)
            return RemoveSet(otherSet);

        bool removedAll = true;
        foreach (var item in items)
        {
            ref var slot = ref FindSlot(item.AsSpan());
            if (Unsafe.IsNullRef(ref slot))
            {
                removedAll = false;
                continue;
            }

            if (slot == count)
                RemoveLast();
            else
                RemoveAt(ref slot);
        }
        // TODO: This is not necessarily correct
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, items));
        return removedAll;
    }

    private bool RemoveSet(ObservableStringSet otherSet)
    {
        // Fast path that avoid rehashing when removing a set of items
        bool removedAll = true;
        for (int i = 0; i < otherSet.count; i++)
        {
            var item = otherSet.array[i];
            var hash = otherSet.hashes[i];
            ref var slot = ref FindSlot(item.AsSpan(), hash);
            if (Unsafe.IsNullRef(ref slot))
            {
                removedAll = false;
                continue;
            }

            if (slot == count)
                RemoveLast();
            else
                RemoveAt(ref slot);
        }
        // TODO: This is not necessarily correct
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, otherSet));
        return removedAll;
    }

    public bool Remove(string item)
    {
        return Remove(item.AsSpan());
    }

    public bool Remove(ReadOnlySpan<char> item)
    {
        ref var slot = ref FindSlot(item);
        if (Unsafe.IsNullRef(ref slot))
            return false;

        var removed = array[slot - 1];
        if (slot == count)
            RemoveLast();
        else
            RemoveAt(ref slot);

        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, removed));

        return true;
    }

    private void RemoveLast()
    {
        count--;

        Array.Clear(buckets, 0, buckets.Length);
        RefillBuckets();

        return;
    }

    private void RemoveAt(ref int slot)
    {
        // Shrink the array to remove the given item
        Array.Copy(array, slot, array, slot - 1, count - slot - 2);
        Array.Copy(hashes, slot, hashes, slot - 1, count - slot - 2);

        //slot = 0;

        count--;

        // Recompute the buckets in the hash set
        Array.Clear(buckets, 0, buckets.Length);
        RefillBuckets();
    }

    private bool Contains(string item, int hash)
    {
        return !Unsafe.IsNullRef(ref FindSlot(item.AsSpan(), hash));
    }

    public bool Contains(string item)
    {
        return !Unsafe.IsNullRef(ref FindSlot(item.AsSpan()));
    }

    public bool Contains(ReadOnlySpan<char> item)
    {
        return !Unsafe.IsNullRef(ref FindSlot(item));
    }

    public void CopyTo(string[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentException(null, nameof(arrayIndex));
        if (array.Length - arrayIndex < count)
            throw new ArgumentOutOfRangeException(nameof(array));

        Array.Copy(this.array, 0, array, arrayIndex, count);
    }

    #region Boolean Operations
    public void ExceptWith(IEnumerable<string> other)
    {
        RemoveRange(other);
    }

    public void IntersectWith(IEnumerable<string> other)
    {
        // TODO: Implement optimised code path for ObservableStringSet, we can take advantage of the existing hash array
        switch (other)
        {
            //case ObservableStringSet stringSet:
            case ICollection<string> collection:
                {
                    List<string> toRemove = [];
                    for (int i = 0; i < count; i++)
                    {
                        var item = array[i];

                        if (!collection.Contains(item))
                            toRemove.Add(item);
                        //Remove(item);
                    }
                    RemoveRange(toRemove);
                }
                break;
            default:
                {
                    List<string> newItems = [];
                    foreach (var item in other)
                        if (Contains(item))
                            newItems.Add(item);

                    Clear();
                    AddRange(newItems);
                }
                break;
        }
    }

    public void SymmetricExceptWith(IEnumerable<string> other)
    {
        // TODO: Implement optimised code path for ObservableStringSet, we can take advantage of the existing hash array
        switch (other)
        {
            //case ObservableStringSet stringSet:
            default:
                {
                    List<string> toRemove = [];
                    List<string> toAdd = [];
                    foreach (var item in other)
                    {
                        if (Contains(item))
                            toRemove.Add(item);
                        else
                            toAdd.Add(item);
                    }
                    RemoveRange(toRemove);
                    AddRange(toAdd);
                }
                break;
        }
    }

    public void UnionWith(IEnumerable<string> other) => AddRange(other);
    #endregion

    #region Boolean Tests
    public bool IsProperSubsetOf(IEnumerable<string> other)
    {
        if (other is ICollection<string> collection
            && Count >= collection.Count)
            return false;

        if (other is ObservableStringSet stringSet)
        {
            for (int i = 0; i < count; i++)
            {
                var item = array[i];
                var hash = hashes[i];
                if (!stringSet.Contains(item, hash))
                    return false;
            }
            return true;
        }

        // Lazy approach to take advantage of fast membership testing
        // probably slower if either set only has a few elements
        return IsProperSubsetOf(new ObservableStringSet(other));
    }

    public bool IsProperSupersetOf(IEnumerable<string> other)
    {
        if (other is ICollection<string> collection
            && Count <= collection.Count)
            return false;

        if (other is ObservableStringSet stringSet)
        {
            for (int i = 0; i < stringSet.count; i++)
            {
                var item = stringSet.array[i];
                var hash = stringSet.hashes[i];
                if (!Contains(item, hash))
                    return false;
            }
            return true;
        }

        int otherCount = 0;
        foreach (var item in other)
        {
            if (!Contains(item))
                return false;
            otherCount++;
        }
        return count > otherCount;
    }

    public bool IsSubsetOf(IEnumerable<string> other)
    {
        if (other is ICollection<string> collection
            && collection.Count > Count)
            return false;

        if (other is ObservableStringSet stringSet)
        {
            for (int i = 0; i < count; i++)
            {
                var item = array[i];
                var hash = hashes[i];
                if (!stringSet.Contains(item, hash))
                    return false;
            }
            return true;
        }

        // Lazy approach to take advantage of fast membership testing
        // probably slower if either set only has a few elements
        return IsSubsetOf(new ObservableStringSet(other));
    }

    public bool IsSupersetOf(IEnumerable<string> other)
    {
        if (other is ICollection<string> collection
            && Count < collection.Count)
            return false;

        if (other is ObservableStringSet stringSet)
        {
            for (int i = 0; i < stringSet.count; i++)
            {
                var item = stringSet.array[i];
                var hash = stringSet.hashes[i];
                if (!Contains(item, hash))
                    return false;
            }
            return true;
        }

        int otherCount = 0;
        foreach (var item in other)
        {
            if (!Contains(item))
                return false;
            otherCount++;
        }
        return count >= otherCount;
    }

    public bool Overlaps(IEnumerable<string> other)
    {
        if (other is ObservableStringSet stringSet)
        {
            for (int i = 0; i < stringSet.count; i++)
            {
                var item = stringSet.array[i];
                var hash = stringSet.hashes[i];
                if (Contains(item, hash))
                    return true;
            }
            return false;
        }

        foreach (var item in other)
            if (Contains(item))
                return true;
        return false;
    }

    public bool SetEquals(IEnumerable<string> other)
    {
        if (other is ICollection<string> collection
            && collection.Count != Count)
            return false;

        if (other is ObservableStringSet stringSet)
        {
            for (int i = 0; i < stringSet.count; i++)
            {
                var item = stringSet.array[i];
                var hash = stringSet.hashes[i];
                if (!Contains(item, hash))
                    return false;
            }
            return true;
        }

        int otherCount = 0;
        foreach (var item in other)
        {
            if (!Contains(item))
                return false;
            otherCount++;
        }
        return otherCount == count;
    }
    #endregion

    public IEnumerator<string> GetEnumerator()
    {
        return new ObservableStringSetEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new ObservableStringSetEnumerator(this);
    }

    public struct ObservableStringSetEnumerator : IEnumerator<string>
    {
        private readonly ObservableStringSet list;
        private readonly int version;
        private int index;
        private string? current;

        internal ObservableStringSetEnumerator(ObservableStringSet list)
        {
            this.list = list;
            version = list.version;
            current = null;
        }

        public readonly string Current => current!;

        readonly object IEnumerator.Current => current!;

        public readonly void Dispose() { }

        public bool MoveNext()
        {
            var _list = list;

            if (version != _list.version)
                throw new InvalidOperationException();

            if ((uint)index < (uint)_list.count)
            {
                current = _list[index];
                index++;
                return true;
            }
            index++;

            return false;
        }

        public void Reset()
        {
            if (version != list.version)
                throw new InvalidOperationException();

            index = 0;
        }
    }
}
