using System;

namespace FreeType.Bindings;

public partial struct FT_PaintRotate_
{
    [NativeTypeName("FT_OpaquePaint")]
    public FT_Opaque_Paint_ paint;

    [NativeTypeName("FT_Fixed")]
    public IntPtr angle;

    [NativeTypeName("FT_Fixed")]
    public IntPtr center_x;

    [NativeTypeName("FT_Fixed")]
    public IntPtr center_y;
}
