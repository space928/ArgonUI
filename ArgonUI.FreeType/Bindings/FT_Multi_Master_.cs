using System.Runtime.CompilerServices;

namespace FreeType.Bindings;

public partial struct FT_Multi_Master_
{
    [NativeTypeName("FT_UInt")]
    public uint num_axis;

    [NativeTypeName("FT_UInt")]
    public uint num_designs;

    [NativeTypeName("FT_MM_Axis[4]")]
    public _axis_e__FixedBuffer axis;

    public partial struct _axis_e__FixedBuffer
    {
        public FT_MM_Axis_ e0;
        public FT_MM_Axis_ e1;
        public FT_MM_Axis_ e2;
        public FT_MM_Axis_ e3;

        public unsafe ref FT_MM_Axis_ this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (FT_MM_Axis_* pThis = &e0)
                {
                    return ref pThis[index];
                }
            }
        }
    }
}
