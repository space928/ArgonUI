using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ArgonUI.Helpers;

/// <summary>
/// A type which allows for fast addition, removal, and contents testing of items. This type 
/// also implements <see cref="INotifyCollectionChanged"/>. 
/// </summary>
[DebuggerDisplay("Count = {Count}")]
public class ObservableStringSet : ICollection<string>, ISet<string>, IReadOnlyCollection<string>, INotifyCollectionChanged//, IReadOnlySet<string>
{
    private string?[] array;
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

    public int Count => count;
    public bool IsReadOnly => false;
    public int Capacity => array.Length;

    private static int GetKeyHash(ReadOnlySpan<char> key)
    {
        //return string.GetHashCode(key);

        uint hash = 2166136261;
        const uint prime = 16777619;

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
            ref Unsafe.As<char, uint>(ref MemoryMarshal.GetReference(key)),
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

        // Appl the final mixer from PCG random
        uint word = ((hash >> unchecked((int)(hash >> 28) + 4)) ^ hash) * 277803737u;
        return unchecked((int)((word >> 22) ^ word));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FastMod(int hash, int mod)
    {
        // https://lemire.me/blog/2016/06/27/a-fast-alternative-to-the-modulo-reduction/
        //return unchecked((int)(((ulong)(uint)hash * (ulong)mod) >> 32));
        // Only works for mods which are powers of 2.
        return hash & mod - 1;
    }

    /// <summary>
    /// Checks if a given entry in the hashtable matches the search value.
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="ind"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool KeyMatch(int hash, int ind, in ReadOnlySpan<char> key)
    {
        if (ind < 0 || hash != hashes[ind])
            return false;

        ref var searchKeyRef = ref MemoryMarshal.GetReference(key);
        // Buckets should only ever point to non-null entries
        string tableKey = array[ind]!;
#if !NETSTANDARD
        ref readonly char tableKeyRef = ref tableKey.GetPinnableReference();
        if (Unsafe.AreSame(in searchKeyRef, in tableKeyRef))
            return true;
        ReadOnlySpan<char> tableKeySpan = tableKey.AsSpan();
#else
        ReadOnlySpan<char> tableKeySpan = tableKey.AsSpan();
        ref char tableKeyRef = ref MemoryMarshal.GetReference(tableKeySpan);
        if (Unsafe.AreSame(ref searchKeyRef, ref tableKeyRef))
            return true;
#endif

        return key.SequenceEqual(tableKeySpan);
    }

    /// <summary>
    /// Checks if a given entry in the hashtable matches the search value.
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="ind"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool KeyMatch(int hash, int ind, string key)
    {
        if (ind < 0 || hash != hashes[ind])
            return false;

        string tableKey = array[ind]!;
#if !NETSTANDARD
        ref readonly char tableKeyRef = ref tableKey.GetPinnableReference();
        ref readonly char searchKeyRef = ref key.GetPinnableReference();
        if (Unsafe.AreSame(in searchKeyRef, in tableKeyRef))
            return true;
        ReadOnlySpan<char> tableKeySpan = tableKey.AsSpan();
        ReadOnlySpan<char> searchKeySpan = key.AsSpan();
#else
        ReadOnlySpan<char> tableKeySpan = tableKey.AsSpan();
        ReadOnlySpan<char> searchKeySpan = key.AsSpan();
        ref char tableKeyRef = ref MemoryMarshal.GetReference(tableKeySpan);
        ref char searchKeyRef = ref MemoryMarshal.GetReference(searchKeySpan);
        if (Unsafe.AreSame(ref searchKeyRef, ref tableKeyRef))
            return true;
#endif

        return searchKeySpan.SequenceEqual(tableKeySpan);
    }

    /// <summary>
    /// Gets a reference to the bucket slot containing the given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="hash"></param>
    /// <returns>A index of the link in <see cref="buckets"/> if found, 
    /// otherwise returns -1.</returns>
    private int FindBucket(ReadOnlySpan<char> item, int hash)
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
                if (KeyMatch(hash, ind - 1, item))
                    return cand;
            }
            else
            {
                return -1;
            }

            unchecked
            {
                cand++;
                cand &= nBuckets - 1;
            }
        } while (cand != slot);

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FindBucket(ReadOnlySpan<char> item)
    {
        int keyHash = GetKeyHash(item);
        return FindBucket(item, keyHash);
    }

    /// <summary>
    /// Attempts to add a link to the given item to the buckets array.
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="item"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    private bool TryPlaceInSet(int hash, string item, int link)
    {
        int nBuckets = buckets.Length;
        int slot = FastMod(hash, nBuckets);
        int cand = slot;
        do
        {
            // Linear probing for a free space, any bucket value <= 0 is a free slot.
            int bucket = buckets[cand];
            if (bucket <= 0)
            {
                buckets[cand] = link + 1; // Indices in the buckets array are 1-based such that 0 can be used as a sentinel.
                return true;
            }
            else if (KeyMatch(hash, bucket - 1, item))
                return false;
            cand++;
            if (cand == nBuckets)
                cand = 0;
        } while (cand != slot);

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefillBuckets()
    {
        for (int i = 0; i < count; i++)
        {
            // All strings in the array from 0-count must be non-null
            var item = array[i]!;
            //var hash = GetKeyHash(item.AsSpan());
            var hash = hashes[i];
            TryPlaceInSet(hash, item, i);
        }
    }

    public void EnsureCapacity(int capacity)
    {
        if (capacity < count)
            capacity = count;

        if (capacity == array.Length)
            return;

#if !NETSTANDARD
        capacity = (int)BitOperations.RoundUpToPowerOf2((uint)(capacity));
#else
        capacity = (int)PolyFill.RoundUpToPowerOf2((uint)(capacity));
#endif

        var nArray = new string[capacity];
        Array.Copy(array, nArray, count);
        array = nArray;

        var nHashes = new int[capacity];
        Array.Copy(hashes, nHashes, count);
        hashes = nHashes;

        // Load factor of 50%
        var nBuckets = new int[capacity * 2];
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
        EnsureCapacity(count + 1);

        if (TryPlaceInSet(hash, item, count))
        {
            version++;
            hashes[count] = hash;
            array[count] = item;
            count++;
            //CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item));
            return true;
        }

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
            EnsureCapacity(ncap);
        }

        bool allAdded = true;
        if (items is ObservableStringSet stringSet)
        {
            // Fast path for string sets to avoid additional hashing
            for (int i = 0; i < stringSet.count; i++)
            {
                var item = stringSet.array[i]!;
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
            var itemSpan = item.AsSpan();
            var hash = GetKeyHash(itemSpan);

            removedAll &= RemoveSilent(itemSpan, hash, out _);
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

            removedAll &= RemoveSilent(item.AsSpan(), hash, out _);
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
        int keyHash = GetKeyHash(item);
        if (RemoveSilent(item, keyHash, out var removed))
        {
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, removed));

            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool RemoveSilent(ReadOnlySpan<char> item, int keyHash, out string? removed)
    {
        // Check the item is in the set, and find it's corresponding bucket
        var bucketInd = FindBucket(item, keyHash);
        removed = default;
        if (bucketInd == -1)
            return false;

        // Remove the entry from the string and hash arrays
        int slot = buckets[bucketInd] - 1;
        removed = array[slot];
        if (slot != count - 1)
        {
            // Shrink the array to remove the given item
            Array.Copy(array, slot + 1, array, slot, count - slot - 1);
            Array.Copy(hashes, slot + 1, hashes, slot, count - slot - 1);
        }
        count--;
        array[count] = null;

        // TODO: Very innefficient, should probably either use the built in HashSet, or port over the free list from StringDict<T>
        Array.Clear(buckets, 0, buckets.Length);
        RefillBuckets();
        return true;

        // Set the link in the bucket to either a 0 (empty, leaf) or -1 (freed, but has probing neighbours)
        /*int maxBucket = buckets.Length - 1; // Should always be 2^n-1
        int idealBucket = keyHash & maxBucket;
        for (int i = 1; i <= maxBucket; i++)
        {
            int nextSlot = buckets[(bucketInd + i) & maxBucket];

            // Next bucket is empty, we can clear our bucket to 0 as well
            if (nextSlot == 0)
                goto ClearBuckets;
            // Next bucket is freed, we should keep probing
            if (nextSlot == -1)
                continue;

            int nextHash = hashes[nextSlot - 1];
            // The probed item is in our bucket, set our slot to freed.
            if ((nextHash & maxBucket) == idealBucket)
            {
                buckets[bucketInd] = -1;
                break;
            }
        ClearBuckets:
            // Clear all freed buckets in this list
            for (int j = i; j >= 0; j--)
                buckets[(bucketInd + j) & maxBucket] = 0;
            break;
        }

        return true;*/
    }

    private bool Contains(string item, int hash)
    {
        return FindBucket(item.AsSpan(), hash) != -1;
    }

    public bool Contains(string item)
    {
        return FindBucket(item.AsSpan()) != -1;
    }

    public bool Contains(ReadOnlySpan<char> item)
    {
        return FindBucket(item) != -1;
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
                        var item = array[i]!;

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
            return IsProperSubsetOf(stringSet);

        // Lazy approach to take advantage of fast membership testing
        // probably slower if either set only has a few elements
        return IsProperSubsetOf(new ObservableStringSet(other));
    }

    /// <inheritdoc cref="IsProperSubsetOf(IEnumerable{string})"/>
    public bool IsProperSubsetOf(ObservableStringSet other)
    {
        if (Count >= other.Count)
            return false;

        for (int i = 0; i < count; i++)
        {
            var item = array[i]!;
            var hash = hashes[i];
            if (!other.Contains(item, hash))
                return false;
        }
        return true;
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
                var item = stringSet.array[i]!;
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
            && Count > collection.Count)
            return false;

        if (other is ObservableStringSet stringSet)
        {
            for (int i = 0; i < count; i++)
            {
                var item = array[i]!;
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
                var item = stringSet.array[i]!;
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
                var item = stringSet.array[i]!;
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
                var item = stringSet.array[i]!;
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
                current = _list.array[index];
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
