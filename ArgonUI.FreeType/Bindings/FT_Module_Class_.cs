using System;

namespace FreeType.Bindings;

public unsafe partial struct FT_Module_Class_
{
    [NativeTypeName("FT_ULong")]
    public UIntPtr module_flags;

    [NativeTypeName("FT_Long")]
    public IntPtr module_size;

    [NativeTypeName("const FT_String *")]
    public sbyte* module_name;

    [NativeTypeName("FT_Fixed")]
    public IntPtr module_version;

    [NativeTypeName("FT_Fixed")]
    public IntPtr module_requires;

    [NativeTypeName("const void *")]
    public void* module_interface;

    [NativeTypeName("FT_Module_Constructor")]
    public IntPtr module_init;

    [NativeTypeName("FT_Module_Destructor")]
    public IntPtr module_done;

    [NativeTypeName("FT_Module_Requester")]
    public IntPtr get_interface;
}
