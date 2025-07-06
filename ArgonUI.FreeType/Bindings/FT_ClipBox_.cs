namespace FreeType.Bindings;

public partial struct FT_ClipBox_
{
    [NativeTypeName("FT_Vector")]
    public FT_Vector_ bottom_left;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ top_left;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ top_right;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ bottom_right;
}
