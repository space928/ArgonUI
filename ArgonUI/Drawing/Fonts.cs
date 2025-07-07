using System;

namespace ArgonUI.Drawing;

/// <summary>
/// A utility class storing static instances of loaded fonts.
/// </summary>
public partial class Fonts
{
    private static Font? notoSans;

    /// <summary>
    /// Gets or sets the default function used to load fonts.
    /// </summary>
    /// <remarks>
    /// This exists as a settable property so that extension libraries can override the 
    /// default font loader to provide more advanced font functionality.
    /// </remarks>
    public static Func<string, Font> DefaultFontLoader { get; set; } = name => BMFont.Load(name);

    /// <summary>
    /// Gets the instance of the default font, which is included with all distributions of ArgonUI.
    /// </summary>
    public static Font Default => NotoSans;
    public static Font NotoSans => notoSans ??= DefaultFontLoader("NotoSans-SemiBold.fnt");
}
