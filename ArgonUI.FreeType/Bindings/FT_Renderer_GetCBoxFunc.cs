using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Renderer_GetCBoxFunc([NativeTypeName("FT_Renderer")] FT_RendererRec_* renderer, [NativeTypeName("FT_GlyphSlot")] FT_GlyphSlotRec_* slot, [NativeTypeName("FT_BBox *")] FT_BBox_* cbox);
