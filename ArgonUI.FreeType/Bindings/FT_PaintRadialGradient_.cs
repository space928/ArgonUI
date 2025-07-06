using System;

namespace FreeType.Bindings;

public partial struct FT_PaintRadialGradient_
{
    [NativeTypeName("FT_ColorLine")]
    public FT_ColorLine_ colorline;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ c0;

    [NativeTypeName("FT_Pos")]
    public IntPtr r0;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ c1;

    [NativeTypeName("FT_Pos")]
    public IntPtr r1;
}
