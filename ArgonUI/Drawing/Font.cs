using ArgonUI.Drawing;
using ArgonUI.Helpers;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Drawing;

[DebuggerDisplay("Font ({Name,nq})")]
public abstract class Font
{
    protected readonly List<FontGlyph> chars = [];
    protected readonly ReadOnlyCollection<FontGlyph> charsRO;
    protected readonly List<FontKerning> kernings = [];
    protected readonly ReadOnlyCollection<FontKerning> kerningsRO;
    protected readonly List<FontPage> pages = [];
    protected readonly ReadOnlyCollection<FontPage> pagesRO;
    protected readonly Dictionary<float, Font> prescaledAlternatives = [];
    protected FrozenDictionary<int, FontGlyph> charsDictFrozen;
    protected FrozenDictionary<int, FrozenDictionary<int, Vector2>> kerningsDictFrozen;

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
    /// <summary>
    /// The number of pixels from the absolute top of the line to the base of the characters.
    /// </summary>
    public int Base { get; protected set; }
    /// <summary>
    /// The name of the OEM charset used (when not unicode).
    /// </summary>
    public string? Charset { get; protected set; }
    /// <summary>
    /// This is the distance in pixels between each line of text.
    /// </summary>
    public int LineHeight { get; protected set; }
    /// <summary>
    /// The height of the texture, normally used to scale the y pos of the character image.
    /// </summary>
    public int ScaleH { get; protected set; }
    /// <summary>
    /// The width of the texture, normally used to scale the x pos of the character image.
    /// </summary>
    public int ScaleW { get; protected set; }
    /// <summary>
    /// The type of signed-distance field (if any) contained in the font's bitmap.
    /// </summary>
    public FontSDFType SDFType { get; protected set; }
    /// <summary>
    /// Set to true if it is the unicode charset.
    /// </summary>
    public bool Unicode { get; protected set; }

    public ReadOnlyCollection<FontGlyph> Chars => charsRO;
    public FrozenDictionary<int, FontGlyph> CharsDict => charsDictFrozen;
    public ReadOnlyCollection<FontKerning> Kernings => kerningsRO;
    public FrozenDictionary<int, FrozenDictionary<int, Vector2>> KerningsDict => kerningsDictFrozen;
    public ReadOnlyCollection<FontPage> Pages => pagesRO;

    public Font()
    {
        pagesRO = new(pages);
        charsRO = new(chars);
        kerningsRO = new(kernings);
        kerningsDictFrozen = FrozenDictionary<int, FrozenDictionary<int, Vector2>>.Empty;
        charsDictFrozen = FrozenDictionary<int, FontGlyph>.Empty;
    }

    /// <summary>
    /// Trys to get a <see cref="Font"/> instance of this font optimised for rendering at the 
    /// given font size.
    /// </summary>
    /// <param name="fontSize">The font size to target (integer values are recommended)</param>
    /// <param name="targetToNotify">If specified and a scaled font isn't available yet, then this 
    /// target's <see cref="UIElements.Abstract.DirtyFlag.Content"/> flag will be set once a scaled 
    /// is available.</param>
    /// <returns>A <see cref="Font"/> instance for that font size or <see langword="this"/> if one doesn't exist.</returns>
    public virtual Font GetScaledFont(float fontSize, UIElements.UIElement? targetToNotify = null)
    {
        return this;
    }

    protected void BuildCharsDict()
    {
        Dictionary<int, FontGlyph> charsDict = [];
        foreach (var ch in chars)
            charsDict.TryAdd(ch.codepoint, ch);

        charsDictFrozen = charsDict.ToFrozenDictionary();
    }

    protected void BuildKerningsDict()
    {
        Dictionary<int, Dictionary<int, Vector2>> kerns = [];
        try
        {
            foreach (var kern in kernings)
            {
                var charKerns = kerns.GetOrAdd(kern.first, () => []);
                charKerns.TryAdd(kern.second, kern.amount);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"Couldn't parse kernings for font '{Name}'.", ex);
        }

        kerningsDictFrozen = kerns.Select(x =>
        new KeyValuePair<int, FrozenDictionary<int, Vector2>>(x.Key, x.Value.ToFrozenDictionary()))
            .ToFrozenDictionary();
    }

    /// <summary>
    /// Measures the approximate amount of space the font will require on screen in pixels.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="size">The font size.</param>
    /// <param name="width">The amount of horizontal stretch applied to this font.</param>
    /// <returns>A vector of the measured width and height.</returns>
    public virtual Bounds2D Measure(string? str, float size, float width = 1)
    {
        if (string.IsNullOrEmpty(str))
            return Bounds2D.Zero;

        /*
        From the text rendering code: 
            float size_x = size * width;
            float x0 = pos.X + c.xOffset * size_x;
            float x1 = pos.X + (c.xOffset + c.width) * size_x;
            float y0 = pos.Y + c.yOffset * size;
            float y1 = pos.Y + (c.yOffset + c.height) * size;
            advance = c.xAdvance * size_x;
         */

        Bounds2D measure = Bounds2D.Zero;
        float fontSize = size / Size;
        float sizeX = fontSize * width;
        Vector2 fontSizeVec = new(sizeX, fontSize);
        Vector2 pos = Vector2.Zero;
        bool first = true;
        char lastChar = '\0';
        foreach (char c in str!)
        {
            int cp = c;
            if (char.IsHighSurrogate(c))
            {
                lastChar = c;
                continue;
            } 
            else if (char.IsLowSurrogate(c))
            {
                cp = char.ConvertToUtf32(lastChar, c);
            }
            var cDef = charsDictFrozen[cp];

            Bounds2D bounds = cDef.rectBounds;
            bounds *= fontSizeVec;
            bounds += pos;
            pos += cDef.advance * fontSizeVec;

            if (first)
            {
                measure = bounds;
                first = false;
            }
            else
            {
                measure = measure.Union(bounds);
            }
        }
        return measure;
    }

    /// <summary>
    /// Measures the approximate amount of space the font will require on screen in pixels.
    /// </summary>
    /// <param name="c">The character to measure.</param>
    /// <param name="size">The font size.</param>
    /// <param name="width">The amount of horizontal stretch applied to this font.</param>
    /// <returns>A vector of the measured width and height.</returns>
    public virtual Bounds2D Measure(char c, float size, float width = 1)
    {
        float fontSize = size / Size;
        Vector2 fontSizeVec = new(fontSize * width, fontSize);
        var cDef = charsDictFrozen[c];
        return cDef.rectBounds * fontSizeVec;
    }
}

public enum FontSDFType
{
    None,
    SDF,
    MSDF
}

public class FontPage
{
    /// <summary>
    /// The page ID.
    /// </summary>
    public int ID { get; protected set; }
    /// <summary>
    /// The texture file name.
    /// </summary>
    public string? TextureFile { get; protected set; }

    public FontPage() { }
}

public struct FontGlyph
{
    /// <summary>
    /// The glyph's UTF-32 codepoint.
    /// </summary>
    public int codepoint;
    /// <summary>
    /// The 16 bit <see cref="char"/> represented by this glyph. Only valid if <see cref="IsBMPChar"/> is <see langword="true"/>.
    /// </summary>
    public char character;
    /// <summary>
    /// Returns <see langword="true"/> if the glyph is in the Unicode basic multilingual plane 
    /// (and as such can be represented by a single <see cref="char"/>).
    /// </summary>
    public readonly bool IsBMPChar
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => codepoint <= 0xFFFF;
        // Checks if the char is a UTF16 surrogate.
        //get => (codepoint & 0xF800) == 0xD800;
    }
    /// <summary>
    /// The bounds of this glyph in the texture atlas (in pixels).
    /// </summary>
    public Bounds2D textureBounds;
    /// <summary>
    /// The bounds of this glyph's drawn rectangle.
    /// </summary>
    public Bounds2D rectBounds;
    /// <summary>
    /// How much the current position should advance after drawing the glyph.
    /// </summary>
    public Vector2 advance;
    /// <summary>
    /// The texture page where the glyph image is found.
    /// </summary>
    public int page;

    public FontGlyph() { }
}

public struct FontKerning
{
    /// <summary>
    /// The first glyph's codepoint.
    /// </summary>
    public int first;
    /// <summary>
    /// The second glyph's codepoint.
    /// </summary>
    public int second;
    /// <summary>
    /// How much the x and y position should be adjusted when drawing the second glyph immediately following the first.
    /// </summary>
    public Vector2 amount;

    public FontKerning() { }
}

[Flags]
public enum FontStyle
{
    Regular,
    Bold = 1 << 0,
    Italic = 1 << 1,
    Underline = 1 << 2,
}
