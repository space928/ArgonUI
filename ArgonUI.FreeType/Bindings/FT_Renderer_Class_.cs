using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_Renderer_Class_
{
    [NativeTypeName("FT_Module_Class")]
    public FT_Module_Class_ root;

    [NativeTypeName("FT_Glyph_Format")]
    public FT_Glyph_Format_ glyph_format;

    [NativeTypeName("FT_Renderer_RenderFunc")]
    public IntPtr render_glyph;

    [NativeTypeName("FT_Renderer_TransformFunc")]
    public IntPtr transform_glyph;

    [NativeTypeName("FT_Renderer_GetCBoxFunc")]
    public IntPtr get_glyph_cbox;

    [NativeTypeName("FT_Renderer_SetModeFunc")]
    public IntPtr set_mode;

    [NativeTypeName("const FT_Raster_Funcs *")]
    public FT_Raster_Funcs_* raster_class;
}
