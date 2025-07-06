using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int FT_Raster_RenderFunc([NativeTypeName("FT_Raster")] FT_RasterRec_* raster, [NativeTypeName("const FT_Raster_Params *")] FT_Raster_Params_* @params);
