using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_Renderer_RenderFunc([NativeTypeName("FT_Renderer")] FT_RendererRec_* renderer, [NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_Render_Mode")] FT_Render_Mode_ mode, [NativeTypeName("const FT_Vector *")] FT_Vector_* origin);
