using System;

namespace FreeType.Bindings;

public partial struct FT_Glyph_Metrics_
{
    [NativeTypeName("FT_Pos")]
    public IntPtr width;

    [NativeTypeName("FT_Pos")]
    public IntPtr height;

    [NativeTypeName("FT_Pos")]
    public IntPtr horiBearingX;

    [NativeTypeName("FT_Pos")]
    public IntPtr horiBearingY;

    [NativeTypeName("FT_Pos")]
    public IntPtr horiAdvance;

    [NativeTypeName("FT_Pos")]
    public IntPtr vertBearingX;

    [NativeTypeName("FT_Pos")]
    public IntPtr vertBearingY;

    [NativeTypeName("FT_Pos")]
    public IntPtr vertAdvance;
}
