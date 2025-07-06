using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int FT_Raster_BitTest_Func(int y, int x, void* user);
