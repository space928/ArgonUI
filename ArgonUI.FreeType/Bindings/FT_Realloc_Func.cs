using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void* FT_Realloc_Func([NativeTypeName("FT_Memory")] FT_MemoryRec_* memory, [NativeTypeName("long")] IntPtr cur_size, [NativeTypeName("long")] IntPtr new_size, void* block);
