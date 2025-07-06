using System;
using System.Runtime.CompilerServices;

namespace FreeType.Bindings;

public unsafe partial struct CID_FaceInfoRec_
{
    [NativeTypeName("FT_String *")]
    public sbyte* cid_font_name;

    [NativeTypeName("FT_Fixed")]
    public IntPtr cid_version;

    [NativeTypeName("FT_Int")]
    public int cid_font_type;

    [NativeTypeName("FT_String *")]
    public sbyte* registry;

    [NativeTypeName("FT_String *")]
    public sbyte* ordering;

    [NativeTypeName("FT_Int")]
    public int supplement;

    [NativeTypeName("PS_FontInfoRec")]
    public PS_FontInfoRec_ font_info;

    [NativeTypeName("FT_BBox")]
    public FT_BBox_ font_bbox;

    [NativeTypeName("FT_ULong")]
    public UIntPtr uid_base;

    [NativeTypeName("FT_Int")]
    public int num_xuid;

    [NativeTypeName("FT_ULong[16]")]
    public _xuid_e__FixedBuffer xuid;

    [NativeTypeName("FT_ULong")]
    public UIntPtr cidmap_offset;

    [NativeTypeName("FT_UInt")]
    public uint fd_bytes;

    [NativeTypeName("FT_UInt")]
    public uint gd_bytes;

    [NativeTypeName("FT_ULong")]
    public UIntPtr cid_count;

    [NativeTypeName("FT_UInt")]
    public uint num_dicts;

    [NativeTypeName("CID_FaceDict")]
    public CID_FaceDictRec_* font_dicts;

    [NativeTypeName("FT_ULong")]
    public UIntPtr data_offset;

    public partial struct _xuid_e__FixedBuffer
    {
        public UIntPtr e0;
        public UIntPtr e1;
        public UIntPtr e2;
        public UIntPtr e3;
        public UIntPtr e4;
        public UIntPtr e5;
        public UIntPtr e6;
        public UIntPtr e7;
        public UIntPtr e8;
        public UIntPtr e9;
        public UIntPtr e10;
        public UIntPtr e11;
        public UIntPtr e12;
        public UIntPtr e13;
        public UIntPtr e14;
        public UIntPtr e15;

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
