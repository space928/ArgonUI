using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Glyph_DoneFunc([NativeTypeName("FT_Glyph")] FT_GlyphRec_* glyph);
