using System.Runtime.InteropServices;

namespace FreeType.Bindings;

public unsafe partial struct BDF_PropertyRec_
{
    [NativeTypeName("BDF_PropertyType")]
    public BDF_PropertyType_ type;

    [NativeTypeName("__AnonymousRecord_ftbdf_L120_C5")]
    public _u_e__Union u;

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _u_e__Union
    {
        [FieldOffset(0)]
        [NativeTypeName("const char *")]
        public sbyte* atom;

        [FieldOffset(0)]
        [NativeTypeName("FT_Int32")]
        public int integer;

        [FieldOffset(0)]
        [NativeTypeName("FT_UInt32")]
        public uint cardinal;
    }
}
