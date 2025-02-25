using ArgonUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

// TODO: Pull up some members of BMFont to font so that we can have a functional abstraction. Until then just use BMFont.
public class Font
{
    /// <summary>
    /// Gets the user readable name of the font.
    /// </summary>
    public string? Name { get; protected set; }
    /// <summary>
    /// Gets the texture associated with the font, used for rendering.
    /// </summary>
    public ArgonTexture? FontTexture { get; protected set; }
    /// <summary>
    /// Gets the style attributes associated with this variant of the font.
    /// </summary>
    public FontStyle FontStyle { get; protected set; }
    /// <summary>
    /// Gets the native size of the font when the texture is used at 1:1 scale.
    /// </summary>
    public int Size { get; protected set; }
}

/// <summary>
/// A utility class storing static instances of loaded fonts.
/// </summary>
public partial class Fonts
{
    private static BMFont? notoSans;

    /// <summary>
    /// Gets the instance of the default font, which is included with all distributions of ArgonUI.
    /// </summary>
    public static BMFont Default => NotoSans;
    public static BMFont NotoSans => notoSans ??= BMFont.Load("NotoSans-SemiBold.fnt");
}

[Flags]
public enum FontStyle
{
    Regular,
    Bold = 1 << 0,
    Italic = 1 << 1,
    Underline = 1 << 2,
}
