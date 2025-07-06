using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_Glyph_PrepareFunc([NativeTypeName("FT_Glyph")] FT_GlyphRec_* glyph, [NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot);
