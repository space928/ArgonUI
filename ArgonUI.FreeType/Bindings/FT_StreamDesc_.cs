using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[StructLayout(LayoutKind.Explicit)]
public unsafe partial struct FT_StreamDesc_
{
    [FieldOffset(0)]
    [NativeTypeName("long")]
    public IntPtr value;

    [FieldOffset(0)]
    public void* pointer;
}
