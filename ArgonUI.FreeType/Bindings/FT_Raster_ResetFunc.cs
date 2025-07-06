using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Raster_ResetFunc([NativeTypeName("FT_Raster")] FT_RasterRec_* raster, [NativeTypeName("unsigned char *")] byte* pool_base, [NativeTypeName("unsigned long")] UIntPtr pool_size);
