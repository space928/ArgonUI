using System;

namespace FreeType.Bindings;

public partial struct TT_Postscript_
{
    [NativeTypeName("FT_Fixed")]
    public IntPtr FormatType;

    [NativeTypeName("FT_Fixed")]
    public IntPtr italicAngle;

    [NativeTypeName("FT_Short")]
    public short underlinePosition;

    [NativeTypeName("FT_Short")]
    public short underlineThickness;

    [NativeTypeName("FT_ULong")]
    public UIntPtr isFixedPitch;

    [NativeTypeName("FT_ULong")]
    public UIntPtr minMemType42;

    [NativeTypeName("FT_ULong")]
    public UIntPtr maxMemType42;

    [NativeTypeName("FT_ULong")]
    public UIntPtr minMemType1;

    [NativeTypeName("FT_ULong")]
    public UIntPtr maxMemType1;
}
