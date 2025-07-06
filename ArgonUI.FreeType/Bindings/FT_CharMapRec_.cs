namespace FreeType.Bindings;

public unsafe partial struct FT_CharMapRec_
{
    [NativeTypeName("FT_Face")]
    public FT_FaceRec_* face;

    [NativeTypeName("FT_Encoding")]
    public FT_Encoding_ encoding;

    [NativeTypeName("FT_UShort")]
    public ushort platform_id;

    [NativeTypeName("FT_UShort")]
    public ushort encoding_id;
}
