using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ArgonUI.Helpers;

public static class StringHash
{
    public static int GetStringHash(this ReadOnlySpan<char> key)
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

        // FNV Hash, a very simple hash, but in my testing performs equally well in quality to the
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
}
