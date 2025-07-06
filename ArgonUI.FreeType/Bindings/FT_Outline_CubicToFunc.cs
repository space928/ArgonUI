using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int FT_Outline_CubicToFunc([NativeTypeName("const FT_Vector *")] FT_Vector_* control1, [NativeTypeName("const FT_Vector *")] FT_Vector_* control2, [NativeTypeName("const FT_Vector *")] FT_Vector_* to, void* user);
