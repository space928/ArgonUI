using System;

namespace FreeType.Bindings;

public partial struct FT_Bitmap_Size_
{
    [NativeTypeName("FT_Short")]
    public short height;

    [NativeTypeName("FT_Short")]
    public short width;

    [NativeTypeName("FT_Pos")]
    public IntPtr size;

    [NativeTypeName("FT_Pos")]
    public IntPtr x_ppem;

    [NativeTypeName("FT_Pos")]
    public IntPtr y_ppem;
}
