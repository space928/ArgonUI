using System;

namespace FreeType.Bindings;

public partial struct FT_BBox_
{
    [NativeTypeName("FT_Pos")]
    public IntPtr xMin;

    [NativeTypeName("FT_Pos")]
    public IntPtr yMin;

    [NativeTypeName("FT_Pos")]
    public IntPtr xMax;

    [NativeTypeName("FT_Pos")]
    public IntPtr yMax;
}
