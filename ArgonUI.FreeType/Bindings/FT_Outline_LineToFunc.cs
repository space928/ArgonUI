using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int FT_Outline_LineToFunc([NativeTypeName("const FT_Vector *")] FT_Vector_* to, void* user);
