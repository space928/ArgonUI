using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Raster_DoneFunc([NativeTypeName("FT_Raster")] FT_RasterRec_* raster);
