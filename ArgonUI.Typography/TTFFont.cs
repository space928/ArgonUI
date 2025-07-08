using ArgonUI.Drawing;
using ArgonUI.Helpers;
using ArgonUI.UIElements;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Font = SixLabors.Fonts.Font;

namespace ArgonUI.Typography;

public class TTFFontFamily : Drawing.Font
{
    private readonly FontFamily family;
    private readonly object scaledFontsLock;
    private readonly SortedRefList<float, TTFFont> scaledFonts;

    internal TTFFontFamily(FontFamily family)
    {
        this.family = family;
        scaledFontsLock = new();
        scaledFonts = [];
        // Create a default
        GetScaledFont(16);
    }

    public override Drawing.Font GetScaledFont(float fontSize, UIElement? targetToNotify = null)
    {
        float computedSize = ComputeStoredFontSize(fontSize);
        lock (scaledFontsLock)
        {
            if (scaledFonts.TryGetValue(computedSize, out var font))
                return font;
        }

        CreateFont(computedSize)
            .ContinueWith(f =>
            {
                lock (scaledFontsLock)
                {
                    scaledFonts.Add(f.Result.Size, (TTFFont)f.Result);
                }
            });

        lock (scaledFontsLock)
        {
            // Return the largest font in the mean time.
            return scaledFonts.GetValueAtIndex(1, true);
        }
    }

    public async Task<Drawing.Font> CreateFont(float size)
    {
        var internalFont = family.CreateFont(size, SixLabors.Fonts.FontStyle.Regular);
        return await Task.Run(() => new TTFFont(this, internalFont));
    }

    /// <summary>
    /// Gets an integer font size which at least as big as the font size requested.
    /// <para/>
    /// For values of <paramref name="fontSize"/> <c><= 16</c> this function simply refturns the <c>Ceil</c> of that value, 
    /// for larger values quadratically spaced integers are returned to prevent needing to store many as high-resolution 
    /// font atlasses.
    /// </summary>
    /// <param name="fontSize">The desired font size.</param>
    /// <returns>A font size at least as big as <paramref name="fontSize"/>.</returns>
    private static float ComputeStoredFontSize(float fontSize)
    {
        if (fontSize <= 16)
            return MathF.Ceiling(fontSize);

        float x = MathF.Ceiling(MathF.Sqrt(fontSize - 15));
        return MathF.Ceiling(x * x + 15);
    }
}

public class TTFFont : Drawing.Font
{
    private readonly TTFFontFamily family;
    private readonly Font internalFont;

    public TTFFontFamily FontFamily => family;

    internal TTFFont(TTFFontFamily family, Font font)
    {
        this.family = family;
        internalFont = font;
        CreateFromInternal();
    }

    public override Drawing.Font GetScaledFont(float fontSize, UIElement? targetToNotify = null)
    {
        if (fontSize == Size)
            return this;

        return family.GetScaledFont(fontSize, targetToNotify);
    }

    private void CreateFromInternal()
    {
        Name = internalFont.Name;
        FontStyle = (internalFont.IsBold ? Drawing.FontStyle.Bold : 0) | (internalFont.IsItalic ? Drawing.FontStyle.Italic : 0);
        float scale = internalFont.FontMetrics.ScaleFactor;
        Size = internalFont.Size;
        Base = (int)(internalFont.FontMetrics.HorizontalMetrics.Ascender * scale);
        //Charset = internalFont.
        LineHeight = (int)(internalFont.FontMetrics.HorizontalMetrics.LineHeight * scale);
        SDFType = FontSDFType.None;
        Unicode = true;
        
        var codepoints = internalFont.FontMetrics.GetAvailableCodePoints();
        foreach (var cp in codepoints)
        {
            if (!internalFont.TryGetGlyphs(cp, out var glyphs))
                continue;

            //glyphs[0].
        }
    }
}
