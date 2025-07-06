using System;

namespace FreeType.Bindings;

public partial struct FT_Raster_Funcs_
{
    [NativeTypeName("FT_Glyph_Format")]
    public FT_Glyph_Format_ glyph_format;

    [NativeTypeName("FT_Raster_NewFunc")]
    public IntPtr raster_new;

    [NativeTypeName("FT_Raster_ResetFunc")]
    public IntPtr raster_reset;

    [NativeTypeName("FT_Raster_SetModeFunc")]
    public IntPtr raster_set_mode;

    [NativeTypeName("FT_Raster_RenderFunc")]
    public IntPtr raster_render;

    [NativeTypeName("FT_Raster_DoneFunc")]
    public IntPtr raster_done;
}
