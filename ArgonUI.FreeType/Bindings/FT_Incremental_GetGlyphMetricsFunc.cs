using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_Incremental_GetGlyphMetricsFunc([NativeTypeName("FT_Incremental")] FT_IncrementalRec_* incremental, [NativeTypeName("FT_UInt")] uint glyph_index, [NativeTypeName("FT_Bool")] byte vertical, [NativeTypeName("FT_Incremental_MetricsRec *")] FT_Incremental_MetricsRec_* ametrics);
