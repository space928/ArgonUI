using System;

namespace FreeType.Bindings;

public partial struct FT_ColorStop_
{
    [NativeTypeName("FT_Fixed")]
    public IntPtr stop_offset;

    [NativeTypeName("FT_ColorIndex")]
    public FT_ColorIndex_ color;
}
