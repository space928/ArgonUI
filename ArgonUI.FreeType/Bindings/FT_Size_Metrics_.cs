using System;

namespace FreeType.Bindings;

public partial struct FT_Size_Metrics_
{
    [NativeTypeName("FT_UShort")]
    public ushort x_ppem;

    [NativeTypeName("FT_UShort")]
    public ushort y_ppem;

    [NativeTypeName("FT_Fixed")]
    public IntPtr x_scale;

    [NativeTypeName("FT_Fixed")]
    public IntPtr y_scale;

    [NativeTypeName("FT_Pos")]
    public IntPtr ascender;

    [NativeTypeName("FT_Pos")]
    public IntPtr descender;

    [NativeTypeName("FT_Pos")]
    public IntPtr height;

    [NativeTypeName("FT_Pos")]
    public IntPtr max_advance;
}
