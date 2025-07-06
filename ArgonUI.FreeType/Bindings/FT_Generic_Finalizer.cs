using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Generic_Finalizer(void* @object);
