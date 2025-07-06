using System;

namespace FreeType.Bindings;

public partial struct FT_Incremental_MetricsRec_
{
    [NativeTypeName("FT_Long")]
    public IntPtr bearing_x;

    [NativeTypeName("FT_Long")]
    public IntPtr bearing_y;

    [NativeTypeName("FT_Long")]
    public IntPtr advance;

    [NativeTypeName("FT_Long")]
    public IntPtr advance_v;
}
