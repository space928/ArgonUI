using System;

namespace FreeType.Bindings;

public partial struct FT_PaintTranslate_
{
    [NativeTypeName("FT_OpaquePaint")]
    public FT_Opaque_Paint_ paint;

    [NativeTypeName("FT_Fixed")]
    public IntPtr dx;

    [NativeTypeName("FT_Fixed")]
    public IntPtr dy;
}
