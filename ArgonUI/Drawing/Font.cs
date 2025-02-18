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
    public string? Name { get; protected set; }
    public ITextureHandle? FontTexture { get; protected set; }
    public FontStyle FontStyle { get; protected set; }
    public int Size { get; protected set; }
}

/// <summary>
/// A utility class storing static instances of loaded fonts.
/// </summary>
public partial class Fonts
{
    private static BMFont? notoSans;

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
