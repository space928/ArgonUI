using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_SpanFunc(int y, int count, [NativeTypeName("const FT_Span *")] FT_Span_* spans, void* user);
