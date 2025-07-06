using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Custom_Log_Handler([NativeTypeName("const char *")] sbyte* ft_component, [NativeTypeName("const char *")] sbyte* fmt, [NativeTypeName("va_list")] sbyte* args);
