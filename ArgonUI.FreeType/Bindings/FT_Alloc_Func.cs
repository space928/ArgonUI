using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void* FT_Alloc_Func([NativeTypeName("FT_Memory")] FT_MemoryRec_* memory, [NativeTypeName("long")] IntPtr size);
