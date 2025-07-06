using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_List_Destructor([NativeTypeName("FT_Memory")] FT_MemoryRec_* memory, void* data, void* user);
