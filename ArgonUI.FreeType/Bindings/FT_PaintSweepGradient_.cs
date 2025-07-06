using System;

namespace FreeType.Bindings;

public partial struct FT_PaintSweepGradient_
{
    [NativeTypeName("FT_ColorLine")]
    public FT_ColorLine_ colorline;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ center;

    [NativeTypeName("FT_Fixed")]
    public IntPtr start_angle;

    [NativeTypeName("FT_Fixed")]
    public IntPtr end_angle;
}
