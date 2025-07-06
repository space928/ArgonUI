using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_Glyph_CopyFunc([NativeTypeName("FT_Glyph")] FT_GlyphRec_* source, [NativeTypeName("FT_Glyph")] FT_GlyphRec_* target);
