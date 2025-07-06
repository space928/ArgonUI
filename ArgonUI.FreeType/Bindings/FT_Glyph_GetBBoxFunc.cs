using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Glyph_GetBBoxFunc([NativeTypeName("FT_Glyph")] FT_GlyphRec_* glyph, [NativeTypeName("FT_BBox *")] FT_BBox_* abbox);
