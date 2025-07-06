using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_Renderer_SetModeFunc([NativeTypeName("FT_Renderer")] FT_RendererRec_* renderer, [NativeTypeName("FT_ULong")] UIntPtr mode_tag, [NativeTypeName("FT_Pointer")] void* mode_ptr);
