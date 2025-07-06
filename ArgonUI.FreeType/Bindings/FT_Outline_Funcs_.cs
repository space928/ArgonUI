using System;

namespace FreeType.Bindings;

public partial struct FT_Outline_Funcs_
{
    [NativeTypeName("FT_Outline_MoveToFunc")]
    public IntPtr move_to;

    [NativeTypeName("FT_Outline_LineToFunc")]
    public IntPtr line_to;

    [NativeTypeName("FT_Outline_ConicToFunc")]
    public IntPtr conic_to;

    [NativeTypeName("FT_Outline_CubicToFunc")]
    public IntPtr cubic_to;

    public int shift;

    [NativeTypeName("FT_Pos")]
    public IntPtr delta;
}
