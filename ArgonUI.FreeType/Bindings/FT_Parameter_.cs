using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_Parameter_
{
    [NativeTypeName("FT_ULong")]
    public UIntPtr tag;

    [NativeTypeName("FT_Pointer")]
    public void* data;
}
