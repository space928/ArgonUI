using System.Runtime.InteropServices;

namespace FreeType.Bindings;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: NativeTypeName("FT_Error")]
public unsafe delegate int FTC_Face_Requester([NativeTypeName("FTC_FaceID")] void* face_id, [NativeTypeName("FT_Library")] FT_LibraryRec_* library, [NativeTypeName("FT_Pointer")] void* req_data, [NativeTypeName("FT_Face *")] FT_FaceRec_** aface);
