using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_Raster_Params_
{
    [NativeTypeName("const FT_Bitmap *")]
    public FT_Bitmap_* target;

    [NativeTypeName("const void *")]
    public void* source;

    public int flags;

    [NativeTypeName("FT_SpanFunc")]
    public IntPtr gray_spans;

    [NativeTypeName("FT_SpanFunc")]
    public IntPtr black_spans;

    [NativeTypeName("FT_Raster_BitTest_Func")]
    public IntPtr bit_test;

    [NativeTypeName("FT_Raster_BitSet_Func")]
    public IntPtr bit_set;

    public void* user;

    [NativeTypeName("FT_BBox")]
    public FT_BBox_ clip_box;
}
