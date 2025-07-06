using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int FT_Raster_NewFunc(void* memory, [NativeTypeName("FT_Raster *")] FT_RasterRec_** raster);
