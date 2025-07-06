using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int SVG_Lib_Preset_Slot_Func([NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_Bool")] byte cache, [NativeTypeName("FT_Pointer *")] void** state);
