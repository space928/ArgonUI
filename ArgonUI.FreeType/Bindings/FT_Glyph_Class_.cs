using System;

namespace FreeType.Bindings;

public partial struct FT_Glyph_Class_
{
    [NativeTypeName("FT_Long")]
    public IntPtr glyph_size;

    [NativeTypeName("FT_Glyph_Format")]
    public FT_Glyph_Format_ glyph_format;

    [NativeTypeName("FT_Glyph_InitFunc")]
    public IntPtr glyph_init;

    [NativeTypeName("FT_Glyph_DoneFunc")]
    public IntPtr glyph_done;

    [NativeTypeName("FT_Glyph_CopyFunc")]
    public IntPtr glyph_copy;

    [NativeTypeName("FT_Glyph_TransformFunc")]
    public IntPtr glyph_transform;

    [NativeTypeName("FT_Glyph_GetBBoxFunc")]
    public IntPtr glyph_bbox;

    [NativeTypeName("FT_Glyph_PrepareFunc")]
    public IntPtr glyph_prepare;
}
