using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Glyph_TransformFunc([NativeTypeName("FT_Glyph")] FT_GlyphRec_* glyph, [NativeTypeName("const FT_Matrix *")] FT_Matrix_* matrix, [NativeTypeName("const FT_Vector *")] FT_Vector_* delta);
