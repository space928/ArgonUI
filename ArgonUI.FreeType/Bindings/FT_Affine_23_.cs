using System;

namespace FreeType.Bindings;

public partial struct FT_Affine_23_
{
    [NativeTypeName("FT_Fixed")]
    public IntPtr xx;

    [NativeTypeName("FT_Fixed")]
    public IntPtr xy;

    [NativeTypeName("FT_Fixed")]
    public IntPtr dx;

    [NativeTypeName("FT_Fixed")]
    public IntPtr yx;

    [NativeTypeName("FT_Fixed")]
    public IntPtr yy;

    [NativeTypeName("FT_Fixed")]
    public IntPtr dy;
}
