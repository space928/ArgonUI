using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void SVG_Lib_Free_Func([NativeTypeName("FT_Pointer *")] void** data_pointer);
