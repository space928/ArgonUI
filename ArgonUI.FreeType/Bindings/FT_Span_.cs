namespace FreeType.Bindings;

public partial struct FT_Span_
{
    public short x;

    [NativeTypeName("unsigned short")]
    public ushort len;

    [NativeTypeName("unsigned char")]
    public byte coverage;
}
