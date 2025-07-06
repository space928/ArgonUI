using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_StreamRec_
{
    [NativeTypeName("unsigned char *")]
    public byte* @base;

    [NativeTypeName("unsigned long")]
    public UIntPtr size;

    [NativeTypeName("unsigned long")]
    public UIntPtr pos;

    [NativeTypeName("FT_StreamDesc")]
    public FT_StreamDesc_ descriptor;

    [NativeTypeName("FT_StreamDesc")]
    public FT_StreamDesc_ pathname;

    [NativeTypeName("FT_Stream_IoFunc")]
    public IntPtr read;

    [NativeTypeName("FT_Stream_CloseFunc")]
    public IntPtr close;

    [NativeTypeName("FT_Memory")]
    public FT_MemoryRec_* memory;

    [NativeTypeName("unsigned char *")]
    public byte* cursor;

    [NativeTypeName("unsigned char *")]
    public byte* limit;
}
