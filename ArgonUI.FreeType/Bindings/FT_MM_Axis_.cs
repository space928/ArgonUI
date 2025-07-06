using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_MM_Axis_
{
    [NativeTypeName("FT_String *")]
    public sbyte* name;

    [NativeTypeName("FT_Long")]
    public IntPtr minimum;

    [NativeTypeName("FT_Long")]
    public IntPtr maximum;
}
