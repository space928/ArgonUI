using System;

namespace FreeType.Bindings;

public partial struct FT_Incremental_FuncsRec_
{
    [NativeTypeName("FT_Incremental_GetGlyphDataFunc")]
    public IntPtr get_glyph_data;

    [NativeTypeName("FT_Incremental_FreeGlyphDataFunc")]
    public IntPtr free_glyph_data;

    [NativeTypeName("FT_Incremental_GetGlyphMetricsFunc")]
    public IntPtr get_glyph_metrics;
}
