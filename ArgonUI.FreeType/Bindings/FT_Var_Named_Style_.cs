using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_Var_Named_Style_
{
    [NativeTypeName("FT_Fixed *")]
    public IntPtr* coords;

    [NativeTypeName("FT_UInt")]
    public uint strid;

    [NativeTypeName("FT_UInt")]
    public uint psid;
}
