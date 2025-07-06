using System;
using System.Runtime.CompilerServices;

namespace FreeType.Bindings;

public partial struct TT_Header_
{
    [NativeTypeName("FT_Fixed")]
    public IntPtr Table_Version;

    [NativeTypeName("FT_Fixed")]
    public IntPtr Font_Revision;

    [NativeTypeName("FT_Long")]
    public IntPtr CheckSum_Adjust;

    [NativeTypeName("FT_Long")]
    public IntPtr Magic_Number;

    [NativeTypeName("FT_UShort")]
    public ushort Flags;

    [NativeTypeName("FT_UShort")]
    public ushort Units_Per_EM;

    [NativeTypeName("FT_ULong[2]")]
    public _Created_e__FixedBuffer Created;

    [NativeTypeName("FT_ULong[2]")]
    public _Modified_e__FixedBuffer Modified;

    [NativeTypeName("FT_Short")]
    public short xMin;

    [NativeTypeName("FT_Short")]
    public short yMin;

    [NativeTypeName("FT_Short")]
    public short xMax;

    [NativeTypeName("FT_Short")]
    public short yMax;

    [NativeTypeName("FT_UShort")]
    public ushort Mac_Style;

    [NativeTypeName("FT_UShort")]
    public ushort Lowest_Rec_PPEM;

    [NativeTypeName("FT_Short")]
    public short Font_Direction;

    [NativeTypeName("FT_Short")]
    public short Index_To_Loc_Format;

    [NativeTypeName("FT_Short")]
    public short Glyph_Data_Format;

    public partial struct _Created_e__FixedBuffer
    {
        public UIntPtr e0;
        public UIntPtr e1;

        public unsafe ref UIntPtr this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (UIntPtr* pThis = &e0)
                {
                    return ref pThis[index];
                }
            }
        }
    }

    public partial struct _Modified_e__FixedBuffer
    {
        public UIntPtr e0;
        public UIntPtr e1;

        public unsafe ref UIntPtr this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (UIntPtr* pThis = &e0)
                {
                    return ref pThis[index];
                }
            }
        }
    }
}
