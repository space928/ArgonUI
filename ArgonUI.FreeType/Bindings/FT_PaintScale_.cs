using System;

namespace FreeType.Bindings;

public partial struct FT_PaintScale_
{
    [NativeTypeName("FT_OpaquePaint")]
    public FT_Opaque_Paint_ paint;

    [NativeTypeName("FT_Fixed")]
    public IntPtr scale_x;

    [NativeTypeName("FT_Fixed")]
    public IntPtr scale_y;

    [NativeTypeName("FT_Fixed")]
    public IntPtr center_x;

    [NativeTypeName("FT_Fixed")]
    public IntPtr center_y;
}
