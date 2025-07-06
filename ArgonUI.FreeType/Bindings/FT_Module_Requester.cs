using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Module_Interface")]
public unsafe delegate void* FT_Module_Requester([NativeTypeName("FT_Module")] FT_ModuleRec_* module, [NativeTypeName("const char *")] sbyte* name);
