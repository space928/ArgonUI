using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_Generic_
{
    public void* data;

    [NativeTypeName("FT_Generic_Finalizer")]
    public IntPtr finalizer;
}
