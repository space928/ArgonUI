using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FT_Stream_CloseFunc([NativeTypeName("FT_Stream")] FT_StreamRec_* stream);
