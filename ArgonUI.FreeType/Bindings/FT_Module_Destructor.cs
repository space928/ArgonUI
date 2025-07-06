using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Module_Destructor([NativeTypeName("FT_Module")] FT_ModuleRec_* module);
