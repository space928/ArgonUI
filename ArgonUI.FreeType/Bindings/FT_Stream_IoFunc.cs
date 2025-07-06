using System;
using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("unsigned long")]
public unsafe delegate UIntPtr FT_Stream_IoFunc([NativeTypeName("FT_Stream")] FT_StreamRec_* stream, [NativeTypeName("unsigned long")] UIntPtr offset, [NativeTypeName("unsigned char *")] byte* buffer, [NativeTypeName("unsigned long")] UIntPtr count);
