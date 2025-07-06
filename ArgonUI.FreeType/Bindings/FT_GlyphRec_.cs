namespace FreeType.Bindings;

public unsafe partial struct FT_GlyphRec_
{
    [NativeTypeName("FT_Library")]
    public FT_LibraryRec_* library;

    [NativeTypeName("const FT_Glyph_Class *")]
    public FT_Glyph_Class_* clazz;

    [NativeTypeName("FT_Glyph_Format")]
    public FT_Glyph_Format_ format;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ advance;
}
