using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_Renderer_TransformFunc([NativeTypeName("FT_Renderer")] FT_RendererRec_* renderer, [NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("const FT_Matrix *")] FT_Matrix_* matrix, [NativeTypeName("const FT_Vector *")] FT_Vector_* delta);
