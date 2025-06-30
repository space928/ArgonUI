using ArgonUI.Helpers;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArgonUI.Drawing;

/// <summary>
/// Implements a reader for the AngelCode BMFont file format.
/// https://www.angelcode.com/products/bmfont/doc/file_format.html
/// </summary>
public class BMFont : Font
{
    private readonly List<BMFontPage> pages = [];
    private readonly List<BMFontChar> chars = [];
    private readonly List<BMFontKerning> kernings = [];
    private readonly ReadOnlyCollection<BMFontPage> pagesRO;
    private readonly ReadOnlyCollection<BMFontChar> charsRO;
    private readonly ReadOnlyCollection<BMFontKerning> kerningsRO;
    private FrozenDictionary<char, FrozenDictionary<char, float>> kerningsDictFrozen;
    private FrozenDictionary<char, BMFontChar> charsDictFrozen;

    /// <summary>
    /// This is the distance in pixels between each line of text.
    /// </summary>
    public int LineHeight { get; private set; }
    /// <summary>
    /// The number of pixels from the absolute top of the line to the base of the characters.
    /// </summary>
    public int Base { get; private set; }
    /// <summary>
    /// The width of the texture, normally used to scale the x pos of the character image.
    /// </summary>
    public int ScaleW { get; private set; }
    /// <summary>
    /// The height of the texture, normally used to scale the y pos of the character image.
    /// </summary>
    public int ScaleH { get; private set; }
    //public int Pages { get; private set; }
    /// <summary>
    /// Set to true if the monochrome characters have been packed into each of the texture 
    /// channels. In this case AlphaChannel describes what is stored in each channel.
    /// </summary>
    public bool Packed { get; private set; }
    /// <summary>
    /// Describes the data stored in the alpha channel.
    /// </summary>
    public BMFontChannel AlphaChannel { get; private set; }
    /// <summary>
    /// Describes the data stored in the alpha channel.
    /// </summary>
    public BMFontChannel RedChannel { get; private set; }
    /// <summary>
    /// Describes the data stored in the alpha channel.
    /// </summary>
    public BMFontChannel GreenChannel { get; private set; }
    /// <summary>
    /// Describes the data stored in the alpha channel.
    /// </summary>
    public BMFontChannel BlueChannel { get; private set; }

    //public string Face { get; private set; }
    //public int Size { get; private set; }
    //public bool Bold { get; private set; }
    //public bool Italic { get; private set; }
    /// <summary>
    /// The name of the OEM charset used (when not unicode).
    /// </summary>
    public string? Charset { get; private set; }
    /// <summary>
    /// Set to true if it is the unicode charset.
    /// </summary>
    public bool Unicode { get; private set; }
    /// <summary>
    /// The font height stretch in 0-1. 100% means no stretch.
    /// </summary>
    public float StretchH { get; private set; }
    /// <summary>
    /// Set to true if smoothing was turned on.
    /// </summary>
    public bool Smooth { get; private set; }
    /// <summary>
    /// The supersampling level used. 1 means no supersampling was used.
    /// </summary>
    public int AA { get; private set; }
    /// <summary>
    /// The padding for each character (up, right, down, left).
    /// </summary>
    public Vector4 Padding { get; private set; }
    /// <summary>
    /// The spacing for each character (horizontal, vertical).
    /// </summary>
    public Vector2 Spacing { get; private set; }
    /// <summary>
    /// The outline thickness for the characters.
    /// </summary>
    public float Outline { get; private set; }

    public BMFontSDFType SDFType { get; private set; }
    public float SDFDistanceRange { get; private set; }

    public ReadOnlyCollection<BMFontPage> Pages => pagesRO;
    public ReadOnlyCollection<BMFontChar> Chars => charsRO;
    public ReadOnlyCollection<BMFontKerning> Kernings => kerningsRO;
    public FrozenDictionary<char, FrozenDictionary<char, float>> KerningsDict => kerningsDictFrozen;
    public FrozenDictionary<char, BMFontChar> CharsDict => charsDictFrozen;

    public BMFont()
    {
        pagesRO = new(pages);
        charsRO = new(chars);
        kerningsRO = new(kernings);
        kerningsDictFrozen = FrozenDictionary<char, FrozenDictionary<char, float>>.Empty;
        charsDictFrozen = FrozenDictionary<char, BMFontChar>.Empty;
    }

    /// <summary>
    /// Measures the approximate amount of space the font will require on screen in pixels.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="size">The font size.</param>
    /// <param name="width">The amount of horizontal stretch applied to this font.</param>
    /// <returns>A vector of the measured width and height.</returns>
    public Bounds2D Measure(string? str, float size, float width = 1)
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
        float xPos = 0;
        bool first = true;
        foreach (char c in str!)
        {
            var cDef = charsDictFrozen[c];

            Vector2 tl = cDef.offset * fontSizeVec;
            Vector2 br = (cDef.offset + cDef.size) * fontSizeVec;
            tl.X += xPos;
            br.X += xPos;
            xPos += cDef.xAdvance * sizeX;

            if (first)
            {
                measure = new(tl, br);
                first = false;
            }
            else
            {
                measure = measure.Union(new(tl, br));
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
    public Bounds2D Measure(char c, float size, float width = 1)
    {
        Vector2 measure = Vector2.Zero;
        float fontSize = size / Size;
        Vector2 fontSizeVec = new(fontSize * width, fontSize);
        var cDef = charsDictFrozen[c];
        Vector2 tl = cDef.offset * fontSizeVec;
        Vector2 br = (cDef.offset + cDef.size) * fontSizeVec;
        return new(tl, br);
    }

    #region Loading
    /// <summary>
    /// Loads a font from a BMFont file.
    /// 
    /// Fonts are in the AngelCode BMFont format: https://www.angelcode.com/products/bmfont/doc/file_format.html
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="xml">Whether the font file is uses the xml format. </param>
    /// <param name="containingAssembly">The assembly to attempt to find the embedded font file in.</param>
    /// <returns></returns>
    public static BMFont Load(string fileName, bool xml = true, Assembly? containingAssembly = null)
    {
        using var stream = ArgonManager.LoadResourceFile(fileName, containingAssembly);
        var bmf = Load(stream, xml);
        return bmf;
    }

    public static BMFont Load(Stream stream, bool xml, Assembly? containingAssembly = null)
    {
        BMFont bmf;
        if (xml)
            bmf = LoadFromXml(stream);
        else
            throw new NotImplementedException();

        if (bmf.pages.Count >= 1)
        {
            string fileName = bmf.pages[0].TextureFile ?? throw new FileNotFoundException($"Attempted to load undefined texture in font '{bmf.Name}'");
            bmf.FontTexture = ArgonTexture.CreateFromFile(fileName, containingAssembly);
        }

        return bmf;
    }

    private static BMFont LoadFromXml(Stream stream)
    {
        using var reader = XmlReader.Create(stream);

        BMFont res = new();

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (reader.Name)
                    {
                        case "common":
                            ReadAttributes(reader, res.ReadCommon);
                            break;
                        case "info":
                            ReadAttributes(reader, res.ReadInfo);
                            break;
                        case "page":
                            res.pages.Add(new(reader));
                            break;
                        case "char":
                            res.chars.Add(new(reader));
                            break;
                        case "kerning":
                            res.kernings.Add(new(reader));
                            break;
                        case "distanceField":
                            ReadAttributes(reader, res.ReadDistanceField);
                            break;
                    }
                    break;
            }
        }

        res.BuildKerningsDict();
        res.BuildCharsDict();

        return res;
    }

    private static void ReadAttributes(XmlReader reader, Action<XmlReader> parser)
    {
        if (!reader.MoveToFirstAttribute())
            return;

        do
        {
            parser(reader);
        } while (reader.MoveToNextAttribute());
    }

    private void ReadCommon(XmlReader reader)
    {
        switch (reader.Name)
        {
            case "lineHeight":
                LineHeight = int.Parse(reader.Value);
                break;
            case "base":
                Base = int.Parse(reader.Value);
                break;
            case "scaleW":
                ScaleW = int.Parse(reader.Value);
                break;
            case "scaleH":
                ScaleH = int.Parse(reader.Value);
                break;
            case "pages":
                break;
            case "packed":
                Packed = reader.Value == "1";
                break;
            case "alphaChnl":
                AlphaChannel = (BMFontChannel)int.Parse(reader.Value);
                break;
            case "redChnl":
                AlphaChannel = (BMFontChannel)int.Parse(reader.Value);
                break;
            case "greenChnl":
                AlphaChannel = (BMFontChannel)int.Parse(reader.Value);
                break;
            case "blueChnl":
                AlphaChannel = (BMFontChannel)int.Parse(reader.Value);
                break;
        }
    }

    private void ReadInfo(XmlReader reader)
    {
        switch (reader.Name)
        {
            case "face":
                Name = reader.Value;
                break;
            case "size":
                Size = int.Parse(reader.Value);
                break;
            case "bold":
                if (reader.Value == "1")
                    FontStyle |= FontStyle.Bold;
                else
                    FontStyle &= ~FontStyle.Bold;
                break;
            case "italic":
                if (reader.Value == "1")
                    FontStyle |= FontStyle.Italic;
                else
                    FontStyle &= ~FontStyle.Italic;
                break;
            case "charset":
                Charset = reader.Value;
                break;
            case "unicode":
                Unicode = reader.Value == "1";
                break;
            case "stretchH":
                StretchH = float.Parse(reader.Value) / 100;
                break;
            case "smooth":
                Smooth = reader.Value == "1";
                break;
            case "aa":
                AA = int.Parse(reader.Value);
                break;
            case "padding":
                {
                    var vals = reader.Value.Split(',');
                    if (vals.Length != 4)
                        throw new InvalidDataException($"BMFont 'padding' expected four values!");
                    Padding = new(float.Parse(vals[0]), float.Parse(vals[1]), float.Parse(vals[2]), float.Parse(vals[3]));
                    break;
                }
            case "spacing":
                {
                    var vals = reader.Value.Split(',');
                    if (vals.Length != 2)
                        throw new InvalidDataException($"BMFont 'spacing' expected two values!");
                    Spacing = new(float.Parse(vals[0]), float.Parse(vals[1]));
                    break;
                }
            case "outline":
                Outline = float.Parse(reader.Value);
                break;
        }
    }

    private void ReadDistanceField(XmlReader reader)
    {
        switch (reader.Name)
        {
            case "fieldType":
                SDFType = reader.Value switch
                {
                    "msdf" => BMFontSDFType.MSDF,
                    "sdf" => BMFontSDFType.SDF,
                    _ => BMFontSDFType.None,
                };
                break;
            case "distanceRange":
                SDFDistanceRange = float.Parse(reader.Value);
                break;
        }
    }

    private void BuildKerningsDict()
    {
        // Temp lookup from id to char
        Dictionary<int, char> charsDict = [];
        foreach (var ch in chars)
            charsDict.TryAdd(ch.id, ch.character);

        Dictionary<char, Dictionary<char, float>> kerns = [];
        try
        {
            foreach (var kern in kernings)
            {
                var charKerns = kerns.GetOrAdd(charsDict[kern.first], () => []);
                charKerns.TryAdd(charsDict[kern.second], kern.amount);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"Couldn't parse kernings for font '{Name}'.", ex);
        }

        kerningsDictFrozen = kerns.Select(x =>
        new KeyValuePair<char, FrozenDictionary<char, float>>(x.Key, x.Value.ToFrozenDictionary()))
            .ToFrozenDictionary();
    }

    private void BuildCharsDict()
    {
        Dictionary<char, BMFontChar> charsDict = [];
        foreach (var ch in chars)
            charsDict.TryAdd(ch.character, ch);

        charsDictFrozen = charsDict.ToFrozenDictionary();
    }
    #endregion
}

public enum BMFontChannel
{
    Glyph,
    Outline,
    GlyphAndOutline,
    Zero,
    One
}

public class BMFontPage
{
    /// <summary>
    /// The page ID.
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// The texture file name.
    /// </summary>
    public string? TextureFile { get; private set; }

    public BMFontPage() { }

    public BMFontPage(XmlReader reader)
    {
        ReadAttributes(reader);
    }

    private void ReadAttributes(XmlReader reader)
    {
        if (!reader.MoveToFirstAttribute())
            return;

        do
        {
            switch (reader.Name)
            {
                case "id":
                    ID = int.Parse(reader.Value);
                    break;
                case "file":
                    TextureFile = reader.Value;
                    break;
            }
        } while (reader.MoveToNextAttribute());
    }
}

public struct BMFontChar
{
    /// <summary>
    /// The character ID.
    /// </summary>
    public int id;
    //public int index;
    /// <summary>
    /// The codepoint this char represents.
    /// </summary>
    public char character;
    /// <summary>
    /// The width and height of the character in pixels.
    /// </summary>
    public Vector2 size;
    /// <summary>
    /// How much the current position should be offset when copying the image from the texture to the screen.
    /// </summary>
    public Vector2 offset;
    /// <summary>
    /// How much the current position should advacne after drawing the character.
    /// </summary>
    public float xAdvance;
    /// <summary>
    /// The texture channel where the character image is found (1 = blue, 2 = green, 4 = red, 8 = alpha, 15 = all channels).
    /// </summary>
    public BMFontCharChannel channel;
    /// <summary>
    /// The top-left position of the character image in the texture.
    /// </summary>
    public Vector2 pos;
    /// <summary>
    /// The texture page where the character image is found.
    /// </summary>
    public int page;

    public BMFontChar() { }

    public BMFontChar(XmlReader reader, string? charset = null)
    {
        ReadAttributes(reader, charset);
    }

    private void ReadAttributes(XmlReader reader, string? charset)
    {
        if (!reader.MoveToFirstAttribute())
            return;

        do
        {
            switch (reader.Name)
            {
                case "id":
                    id = int.Parse(reader.Value);
                    break;
                case "x":
                    pos.X = int.Parse(reader.Value);
                    break;
                case "y":
                    pos.Y = int.Parse(reader.Value);
                    break;
                case "width":
                    size.X = int.Parse(reader.Value);
                    break;
                case "height":
                    size.Y = int.Parse(reader.Value);
                    break;
                case "xoffset":
                    offset.X = int.Parse(reader.Value);
                    break;
                case "yoffset":
                    offset.Y = int.Parse(reader.Value);
                    break;
                case "xadvance":
                    xAdvance = int.Parse(reader.Value);
                    break;
                case "page":
                    page = int.Parse(reader.Value);
                    break;
                case "chnl":
                    channel = (BMFontCharChannel)int.Parse(reader.Value);
                    break;
                case "char":
                    if (reader.Value.Length == 1)
                        character = reader.Value[0];
                    break;
            }
        } while (reader.MoveToNextAttribute());

        if (character == default(char) && id != 0)
        {
            // If the character wasn't specified via the 'char' extension attribute,
            // try decoding it using the given encoding.
            var encoding = Encoding.UTF8;
            try
            {
                encoding = Encoding.GetEncoding(charset!);
            }
            catch { }
#if NETSTANDARD
            unsafe
            {
                fixed (int* bytes = &id)
                fixed (char* chr = &character)
                {
                    encoding.GetChars((byte*)bytes, sizeof(int), chr, 1);
                }
            }
#else
            var bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref id, 1));
            var chars = MemoryMarshal.CreateSpan(ref character, 1);
            encoding.TryGetChars(bytes, chars, out int _);
#endif
        }
    }
}

public struct BMFontKerning
{
    /// <summary>
    /// The first character ID.
    /// </summary>
    public int first;
    /// <summary>
    /// The second character ID.
    /// </summary>
    public int second;
    /// <summary>
    /// How much the x position should be adjusted when drawing the second character immediately following the first.
    /// </summary>
    public float amount;

    public BMFontKerning() { }

    public BMFontKerning(XmlReader reader)
    {
        ReadAttributes(reader);
    }

    private void ReadAttributes(XmlReader reader)
    {
        if (!reader.MoveToFirstAttribute())
            return;

        do
        {
            switch (reader.Name)
            {
                case "first":
                    first = int.Parse(reader.Value);
                    break;
                case "second":
                    second = int.Parse(reader.Value);
                    break;
                case "amount":
                    amount = float.Parse(reader.Value);
                    break;
            }
        } while (reader.MoveToNextAttribute());
    }
}

[Flags]
public enum BMFontCharChannel
{
    None,
    Blue = 1 << 0,
    Green = 1 << 1,
    Red = 1 << 2,
    Alpha = 1 << 3,
    All = Red | Green | Blue | Alpha
}

public enum BMFontSDFType
{
    None,
    SDF,
    MSDF
}
