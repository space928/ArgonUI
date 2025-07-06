using System;

namespace FreeType.Bindings;

public partial struct FT_Size_RequestRec_
{
    [NativeTypeName("FT_Size_Request_Type")]
    public FT_Size_Request_Type_ type;

    [NativeTypeName("FT_Long")]
    public IntPtr width;

    [NativeTypeName("FT_Long")]
    public IntPtr height;

    [NativeTypeName("FT_UInt")]
    public uint horiResolution;

    [NativeTypeName("FT_UInt")]
    public uint vertResolution;
}
