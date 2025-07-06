using System;

namespace FreeType.Bindings;

public partial struct CID_FaceDictRec_
{
    [NativeTypeName("PS_PrivateRec")]
    public PS_PrivateRec_ private_dict;

    [NativeTypeName("FT_UInt")]
    public uint len_buildchar;

    [NativeTypeName("FT_Fixed")]
    public IntPtr forcebold_threshold;

    [NativeTypeName("FT_Pos")]
    public IntPtr stroke_width;

    [NativeTypeName("FT_Fixed")]
    public IntPtr expansion_factor;

    [NativeTypeName("FT_Byte")]
    public byte paint_type;

    [NativeTypeName("FT_Byte")]
    public byte font_type;

    [NativeTypeName("FT_Matrix")]
    public FT_Matrix_ font_matrix;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ font_offset;

    [NativeTypeName("FT_UInt")]
    public uint num_subrs;

    [NativeTypeName("FT_ULong")]
    public UIntPtr subrmap_offset;

    [NativeTypeName("FT_UInt")]
    public uint sd_bytes;
}
