using System;

namespace FreeType.Bindings;

public partial struct FT_Vector_
{
    [NativeTypeName("FT_Pos")]
    public IntPtr x;

    [NativeTypeName("FT_Pos")]
    public IntPtr y;
}
