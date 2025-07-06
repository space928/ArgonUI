using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_SvgGlyphRec_
{
    [NativeTypeName("FT_GlyphRec")]
    public FT_GlyphRec_ root;

    [NativeTypeName("FT_Byte *")]
    public byte* svg_document;

    [NativeTypeName("FT_ULong")]
    public UIntPtr svg_document_length;

    [NativeTypeName("FT_UInt")]
    public uint glyph_index;

    [NativeTypeName("FT_Size_Metrics")]
    public FT_Size_Metrics_ metrics;

    [NativeTypeName("FT_UShort")]
    public ushort units_per_EM;

    [NativeTypeName("FT_UShort")]
    public ushort start_glyph_id;

    [NativeTypeName("FT_UShort")]
    public ushort end_glyph_id;

    [NativeTypeName("FT_Matrix")]
    public FT_Matrix_ transform;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ delta;
}
