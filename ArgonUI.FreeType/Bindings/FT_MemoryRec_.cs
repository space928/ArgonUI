using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_MemoryRec_
{
    public void* user;

    [NativeTypeName("FT_Alloc_Func")]
    public IntPtr alloc;

    [NativeTypeName("FT_Free_Func")]
    public IntPtr free;

    [NativeTypeName("FT_Realloc_Func")]
    public IntPtr realloc;
}
