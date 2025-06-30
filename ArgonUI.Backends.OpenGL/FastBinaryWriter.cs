using Silk.NET.OpenGL;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ArgonUI.Backends.OpenGL;

public ref struct FastBinaryWriter<T> where T : unmanaged
{
#if !NETSTANDARD
    private readonly ref byte ptr;
#else
    private readonly Span<byte> data;
#endif
    private nint offset;
    
    public readonly int Offset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => unchecked((int)offset);
    }

    public FastBinaryWriter(T[] buff, int offset)
    {
#if !NETSTANDARD
        ptr = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(buff));
#else
        data = MemoryMarshal.AsBytes(buff.AsSpan());
#endif
        this.offset = offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<TValue>(in TValue value)
    {
#if !NETSTANDARD
        Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref ptr, offset), value);
#else
        Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(data), offset), value);
#endif
        offset += Unsafe.SizeOf<TValue>();
    }
}

internal static class FastBinaryWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FastBinaryWriter<T> GetBinaryWriter<T>(this T[] array, int offset)
        where T : unmanaged
    {
        return new FastBinaryWriter<T>(array, offset);
    }

    public static unsafe void Write<T, TDst>(this T val, TDst[] dst, ref int offset)
        where T : struct
        where TDst : unmanaged
    {
#if !NETSTANDARD
        Unsafe.WriteUnaligned(
            ref Unsafe.AddByteOffset(
            ref Unsafe.As<TDst, byte>(
                ref MemoryMarshal.GetArrayDataReference(dst)), offset),
            val);
        offset += Unsafe.SizeOf<T>();
#else
        fixed (TDst* ptr = dst)
        {
            Unsafe.WriteUnaligned((byte*)ptr + offset, val);
        }
#endif
    }
}

// This JITs really well, but it's a bit ugly to use...
#if false
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly unsafe struct FasterBinaryWriter<T1, T2, T3> 
    where T1 : unmanaged 
    where T2 : unmanaged
    where T3 : unmanaged
{
    private readonly T1 a;
    private readonly T2 b;
    private readonly T3 c;

    public readonly int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Unsafe.SizeOf<FasterBinaryWriter<T1, T2, T3>>();
    }

    public FasterBinaryWriter(uint[] dst, int offset, T1 a, T2 b, T3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        Unsafe.WriteUnaligned(
            ref Unsafe.AddByteOffset(
            ref Unsafe.As<uint, byte>(
                ref MemoryMarshal.GetArrayDataReference(dst)), offset), 
            this);
    }
}
#endif
