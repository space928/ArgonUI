using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Incremental_FreeGlyphDataFunc([NativeTypeName("FT_Incremental")] FT_IncrementalRec_* incremental, [NativeTypeName("FT_Data *")] FT_Data_* data);
