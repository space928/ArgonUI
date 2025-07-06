using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_Incremental_GetGlyphDataFunc([NativeTypeName("FT_Incremental")] FT_IncrementalRec_* incremental, [NativeTypeName("FT_UInt")] uint glyph_index, [NativeTypeName("FT_Data *")] FT_Data_* adata);
