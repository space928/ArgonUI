using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int SVG_Lib_Init_Func([NativeTypeName("FT_Pointer *")] void** data_pointer);
