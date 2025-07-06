using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int SVG_Lib_Render_Func([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_Pointer *")] void** data_pointer);
