using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FT_List_Iterator([NativeTypeName("FT_ListNode")] FT_ListNodeRec_* node, void* user);
