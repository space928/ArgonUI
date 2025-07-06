using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int FT_Raster_SetModeFunc([NativeTypeName("FT_Raster")] FT_RasterRec_* raster, [NativeTypeName("unsigned long")] UIntPtr mode, void* args);
