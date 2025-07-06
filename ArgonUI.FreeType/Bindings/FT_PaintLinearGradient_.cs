namespace FreeType.Bindings;

public partial struct FT_PaintLinearGradient_
{
    [NativeTypeName("FT_ColorLine")]
    public FT_ColorLine_ colorline;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ p0;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ p1;

    [NativeTypeName("FT_Vector")]
    public FT_Vector_ p2;
}
