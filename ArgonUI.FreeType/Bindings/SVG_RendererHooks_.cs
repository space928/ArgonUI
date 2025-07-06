using System;

namespace FreeType.Bindings;

public partial struct SVG_RendererHooks_
{
    [NativeTypeName("SVG_Lib_Init_Func")]
    public IntPtr init_svg;

    [NativeTypeName("SVG_Lib_Free_Func")]
    public IntPtr free_svg;

    [NativeTypeName("SVG_Lib_Render_Func")]
    public IntPtr render_svg;

    [NativeTypeName("SVG_Lib_Preset_Slot_Func")]
    public IntPtr preset_slot;
}
