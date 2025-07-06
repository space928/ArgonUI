using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_Var_Axis_
{
    [NativeTypeName("FT_String *")]
    public sbyte* name;

    [NativeTypeName("FT_Fixed")]
    public IntPtr minimum;

    [NativeTypeName("FT_Fixed")]
    public IntPtr def;

    [NativeTypeName("FT_Fixed")]
    public IntPtr maximum;

    [NativeTypeName("FT_ULong")]
    public UIntPtr tag;

    [NativeTypeName("FT_UInt")]
    public uint strid;
}
